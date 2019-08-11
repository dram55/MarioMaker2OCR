﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Runtime.CompilerServices;


namespace MarioMaker2OCR
{
    class VideoProcessor : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool disposed = false; // Flag to indicate the object has been disposed
        public const int NO_DEVICE = -1; // Constant indicating that no video device was used

        private VideoCapture cap;        // EmguCV VideoCapture device object
        private int deviceId;            // Device id of the video capture device
        private Size frameSize;          // Contains the Size() object for the frame, needed for frameBuffer_tick to create the iamge
        private Thread processorThread;  // The thread performing the main video processing
        private System.Threading.Timer frameBufferTimer; // Timer used to fill the frame buffer
        private Image<Bgr, byte>[] frameBuffer = new Image<Bgr, byte>[16]; // Frame buffer that is passed to events
        private (bool IsBlack, bool IsClear, bool ShouldStop) flags = (false, false, false); // Contains flags indicating the current status of the video processor

        private const int FRAME_BUFFER_INTERVAL = 250; // put frame in buffer every 250ms

        /// <summary>
        /// Essentially copied from https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Essentially copied from https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                if(frameBufferTimer != null) frameBufferTimer.Dispose();
                if(cap != null) cap.Dispose();
            }
            disposed = true;
        }

        public VideoProcessor(VideoCapture video)
        {
            this.deviceId = NO_DEVICE;
            this.cap = video;
            this.frameSize = getCaptureInfo(video).resolution;
        }

        public VideoProcessor(int device, Size resolution)
        {
            this.deviceId = device;
            this.cap = createCaptureDevice(device, resolution);
            this.frameSize = getCaptureInfo(this.cap).resolution;
        }
        /// <summary>
        /// Starts the main video processing loop
        /// </summary>
        public void Start(bool blocking=false)
        {
            clearFrameBuffer();
            flags.ShouldStop = false;

            processorThread = new Thread(new ThreadStart(processingLoop));
            processorThread.Start();


            // No device means this isn't a real-time capture, so the loop will call _tick
            if(deviceId != NO_DEVICE)
            {
                frameBufferTimer = new System.Threading.Timer(frameBuffer_tick);
                frameBufferTimer.Change(0, FRAME_BUFFER_INTERVAL);
            }

            if(blocking)
            {
                processorThread.Join();
            }
        }

        /// <summary>
        /// Returns once the processing thread has been killed.
        /// </summary>
        public void Stop()
        {
            frameBufferTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            frameBufferTimer = null;
            if(processorThread != null)
            {
                flags.ShouldStop = true;
                while(processorThread.IsAlive)
                {
                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// Grabs a frame for the frame buffer every tick.
        /// </summary>
        public void frameBuffer_tick(object sender=null)
        {
            try
            {
                if (cap == null) return;
                if (flags.IsBlack) return;

                Image<Bgr, byte> frame = new Image<Bgr, byte>(frameSize);
                cap.Retrieve(frame);
                if (frame.Cols == 0) return; // XXX: happens if the tick occures before device is ready

                (frameBuffer[frameBuffer.Length - 1])?.Dispose();
                for (int i = frameBuffer.Length - 1; i > 0; i--)
                {
                    frameBuffer[i] = frameBuffer[i - 1];
                }
                frameBuffer[0] = frame;

                VideoEventArgs args = new VideoEventArgs();
                args.currentFrame = frame.Clone();
                onNewFrame(args);
            }
            catch(Exception ex)
            {
                log.Debug("Error in frame buffer tick");
                log.Error(ex);
            }
        }

        /// <summary>
        /// Clears the existing framebuffer and disposes the current capture device
        /// </summary>
        private void clearFrameBuffer()
        {
            for (int i = 0; i < frameBuffer.Length; i++)
            {
                frameBuffer[i]?.Dispose();
                frameBuffer[i] = null;
            }
        }

        /// <summary>
        /// Initializes the capture device and setups the object's state
        /// </summary>
        /// <param name="device"></param>
        /// <param name="captureResolution"></param>
        private VideoCapture createCaptureDevice(int device, Size captureResolution)
        {
            // Since we are only targeting Windows users, forcing DShow should be a safe choice, this is required for SplitCam to be readable
            var newcap = new VideoCapture(device, VideoCapture.API.DShow);
            newcap.SetCaptureProperty(CapProp.FrameHeight, captureResolution.Height);
            newcap.SetCaptureProperty(CapProp.FrameWidth, captureResolution.Width);

            var info = getCaptureInfo(newcap);
            if(info.channels != 3)
            {
                throw new Exception(String.Format("Unexcepted channel count: {0}", info.channels));
            }

            return newcap;
        }

        /// <summary>
        /// Gets some basic information about the video capture
        /// </summary>
        /// <param name="video"></param>
        /// <returns>The resolution as Size() and an int reflecting the number of channels</returns>
        private (Size resolution, int channels) getCaptureInfo(VideoCapture video)
        {
            Mat tmp = new Mat();
            video.Retrieve(tmp);

            if (tmp.Bitmap == null)
            {
                throw new Exception("Failed to get image from video device");
            }

            return (new Size(tmp.Width, tmp.Height), tmp.NumberOfChannels);
        }




        /// <summary>
        /// Main video processing loop on the video capture device which should already be initalized
        /// </summary>
        public void processingLoop()
        {
            // Create a Mat that uses our preallocated data block so it can be used without a conversion to Image<>
            // Allocated before the loops so we don't leak memory every time the capture devices crashes
            byte[] data = new byte[frameSize.Width * frameSize.Height * 3];
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Mat currentFrame = new Mat(frameSize, DepthType.Cv8U, 3, dataHandle.AddrOfPinnedObject(), frameSize.Width * 3);

            int skip = frameSize.Width / 50;
            DateTime blackStart = DateTime.Now;

            while (true)
            {
                try
                {
                    if(cap == null && deviceId != NO_DEVICE)
                    {
                        this.cap = createCaptureDevice(deviceId, frameSize);
                    }

                    flags.IsBlack = false;
                    flags.IsClear = false;
                    bool WaitForClearStats = false;
                    while (true)
                    {
                        if(deviceId == NO_DEVICE)
                        {
                            cap.Read(currentFrame);
                            if (currentFrame.IsEmpty) return;
                            data = currentFrame.GetRawData();
                            double fps = cap.GetCaptureProperty(CapProp.Fps);
                            double i = cap.GetCaptureProperty(CapProp.PosFrames);
                            if (i % (int)Math.Floor(fps / (1000/FRAME_BUFFER_INTERVAL)) == 0) frameBuffer_tick();
                        }
                        else
                        {
                            cap.Retrieve(currentFrame);
                        }

                        Dictionary<int, int> hues = getHues(data, frameSize, skip);

                        if (flags.IsBlack)
                        {
                            flags.IsBlack = isBlackFrame(hues);
                            if(!flags.IsBlack)
                            {
                                BlackScreenEventArgs args = new BlackScreenEventArgs();
                                args.frameBuffer = copyFrameBuffer();
                                args.currentFrame = getLevelScreenImageFromBuffer(args.frameBuffer);
                                args.seconds = DateTime.Now.Subtract(blackStart).TotalMilliseconds/1000;
                                onBlackScreen(args);
                            }
                        }
                        else if (flags.IsClear)
                        {

                            flags.IsClear = isClearFrame(hues);
                            if (!flags.IsClear) WaitForClearStats = true;
                        }
                        else if (WaitForClearStats && isClearWithStatsScreen(hues))
                        {
                            log.Info("Have clear screen");
                            // HACK: Apart from taking up more CPU to do a comparision like the Level Select screen this is the best solution imo
                            // Match happens during transition, so 500ms is long enough to get to the screen, but not long enough to exit and miss it.
                            Thread.Sleep(593);
                            cap.Retrieve(currentFrame);

                            ClearScreenEventArgs args = new ClearScreenEventArgs();
                            args.currentFrame = currentFrame.Clone().ToImage<Bgr, Byte>();

                            // Check to see if this is the clear screen with comments on - things are positioned differently.
                            // Top of screen is yellow if comments are on
                            Size topOfScreen = new Size(frameSize.Width, frameSize.Height / 6);
                            Dictionary<int, int> topHues = getHues(data, topOfScreen, skip);
                            if (isMostlyYellow(topHues))
                                args.commentsEnabled = true;


                            onClearScreen(args);
                        }
                        else if (isBlackFrame(hues))
                        {
                            flags.IsBlack = true;
                            WaitForClearStats = false; // XXX: If we get a black screen and this is true, something weird is going on
                            blackStart = DateTime.Now;
                        }
                        else if (isClearFrame(hues))
                        {
                            flags.IsClear = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    cap?.Dispose();
                    cap = null;
                    log.Debug("Exception in main procesisng loop");
                    log.Error(ex);
                    Thread.Sleep(5000);
                }
            }
        }

        /// <summary>
        /// Gets the percent of the screen that the hue covers.
        /// </summary>
        /// <param name="hues">Hue dictionary from `getHues`</param>
        /// <param name="hue">Specific hue to look for</param>
        /// <param name="fuzzy">Performs a fuzzy search looking for hues with +/- this argument</param>
        /// <returns>The percentage of the screen covered by the hue</returns>
        public static int getColorCoverage(Dictionary<int,int> hues, int hue, int fuzzy=0)
        {
            int sum = 0;
            if (hues.ContainsKey(hue)) sum += hues[hue];
            for (int i=1;i<=fuzzy;i++)
            {
                if (hues.ContainsKey(hue + i)) sum += hues[hue + i];
                if (hues.ContainsKey(hue - i)) sum += hues[hue - i];
            }
            return sum;
        }

        /// <summary>
        /// Finds the most dominate hue.
        /// </summary>
        /// <param name="hues">Hue dictionary from `getHues`</param>
        /// <returns>Returns the key to the most dominate hue</returns>
        public static int getPrimaryColor(Dictionary<int, int> hues)
        {
            int maxVal = -1;
            int maxHue = -1;
            foreach(var h in hues)
            {
                if(h.Value > maxVal)
                {
                    maxVal = h.Value;
                    maxHue = h.Key;
                }
            }
            return maxHue;
        }

        /// <summary>
        /// Scans an image for all unique hues.
        /// </summary>
        /// <param name="data">Array of pixel data</param>
        /// <param name="frameSize">Size of the frame to be scanned</param>
        /// <param name="skip">Scans only every `skip` pixels.</param>
        /// <returns>Returns a dictionary containing the hue as a key and the percent of the pixeled scanned that matched that hue</returns>
        public static Dictionary<int, int> getHues(byte[] data, Size frameSize, int skip=20)
        {
            int offset;
            Color current;
            Dictionary<int, int> allhues = new Dictionary<int, int>();
            int total = 0;
            for (int x = 0; x < frameSize.Width; x += skip)
            {
                for (int y = 0; y < frameSize.Height; y += skip)
                {
                    offset = (x * 3) + (y * frameSize.Width * 3);
                    current = Color.FromArgb(data[offset + 2], data[offset + 1], data[offset + 0]);
                    int hue = (int)Math.Floor(current.GetHue());
                    if(!allhues.ContainsKey(hue))
                    {
                        allhues.Add(hue, 0);
                    }
                    allhues[hue] = allhues[hue] + 1;
                    total++;
                }
            }

            List<int> keys = new List<int>(allhues.Keys);
            foreach (var key in keys)
            {
                decimal v = (decimal) allhues[key];
                decimal percent = (v / total) * 100m;
                allhues[key] = Decimal.ToInt32(percent);
            }
            return allhues;
        }

        /// <summary>
        /// Created a new copy of all the frames currently in the frame buffer
        /// </summary>
        /// <returns>Returns a new frameBuffer array that won't be updated or disposed.</returns>
        private Image<Bgr, byte>[] copyFrameBuffer()
        {
            Image<Bgr, byte>[] cloned = new Image<Bgr, byte>[frameBuffer.Length];
            for(int i=0;i<frameBuffer.Length; i++)
            {
                cloned[i] = frameBuffer[i]?.Clone();
            }
            return cloned;
        }

        /// <summary>
        /// Determines if a given hue dictionary is from a black frame
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isBlackFrame(Dictionary<int, int> hues)
        {
            return getColorCoverage(hues, 0) > 98;
        }

        /// <summary>
        /// Determines if a given hue dictionry is from a course cleared frame
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isClearFrame(Dictionary<int, int> hues)
        {
            int primary = getPrimaryColor(hues);
            return hues[primary] > 95 && Math.Abs(50 - primary) < 10;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isClearWithStatsScreen(Dictionary<int, int> hues)
        {
            int primary = getPrimaryColor(hues);
            return Math.Abs(50 - primary) < 10 && (hues[primary] > 70 && hues[primary] < 80);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isMostlyYellow(Dictionary<int, int> hues)
        {
            int primary = getPrimaryColor(hues);
            return hues[primary] > 88 && Math.Abs(50 - primary) < 10;
        }


        /// <summary>
        /// Event that fires off whenever a black screen is detected
        /// </summary>
        public event EventHandler<BlackScreenEventArgs> BlackScreen;
        protected virtual void onBlackScreen(BlackScreenEventArgs e)
        {
            if (frameBuffer[0] == null) return;
            BlackScreen?.Invoke(this, e);
            for (int i = 0; i < frameBuffer.Length; i++)
            {
                frameBuffer[i]?.Dispose();
                frameBuffer[i] = null;
            }
        }

        /// <summary>
        /// Event fires off whenever a clear screen is detected
        /// </summary>
        public event EventHandler<ClearScreenEventArgs> ClearScreen;
        protected virtual void onClearScreen(ClearScreenEventArgs e)
        {
            if (frameBuffer[0] == null) return;
            ClearScreen?.Invoke(this, e);
            for (int i = 0; i < frameBuffer.Length; i++)
            {
                frameBuffer[i]?.Dispose();
                frameBuffer[i] = null;
            }
        }

        /// <summary>
        /// Fires off every time the frameBuffer adds a new frame.
        /// </summary>
        public event EventHandler<VideoEventArgs> NewFrame;
        protected virtual void onNewFrame(VideoEventArgs e)
        {
            NewFrame?.Invoke(this, e);
            e.currentFrame.Dispose();
        }
        public class VideoEventArgs : EventArgs
        {
            public Image<Bgr, byte>[] frameBuffer;
            public Image<Bgr, byte> currentFrame;
        }

        public class BlackScreenEventArgs: VideoEventArgs
        {
            public double seconds;
        }

        public class ClearScreenEventArgs : VideoEventArgs
        {
            public bool commentsEnabled;
        }

        /// <summary>
        /// Inspects passed in buffer and returns the best guess of level screen.
        /// 
        /// On a full buffer this is 5 frames in. But you must also account that the buffer
        /// may not be full.
        /// </summary>
        /// <returns></returns>
        private Image<Bgr, byte> getLevelScreenImageFromBuffer(Image<Bgr, byte>[] buffer)
        {
            Image<Bgr, byte> returnImage = null;

            int nullFrameIdx = Array.IndexOf(buffer, null);

            // buffer is full
            if (nullFrameIdx == -1)
                returnImage = buffer[buffer.Length - 5];
            else
            {
                // From testing - more frames in the buffer, the further back you can dig.
                if (nullFrameIdx <= 1)
                    returnImage = buffer[0];
                else if (nullFrameIdx < 6)
                    returnImage = buffer[nullFrameIdx - 1];
                else
                    returnImage = buffer[nullFrameIdx - 5];
            }

            return returnImage;
        }
    }
}