using System;
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


namespace MarioMaker2OCR
{
    class VideoProcessor : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int deviceId;
        private Size resolution;
        private VideoCapture cap;
        private const int frameBufferLength = 10;
        private Image<Bgr, byte>[] frameBuffer = new Image<Bgr, byte>[frameBufferLength];
        private System.Threading.Timer frameBufferTimer;
        private Size frameSize; //frameBuffer_tick needs to know this
        private Thread processorThread;

        bool disposed = false;

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

        public VideoProcessor(int device, Size resolution)
        {
            this.deviceId = device;
            this.resolution = resolution;
        }
        /// <summary>
        /// Starts the main video processing loop
        /// </summary>
        public void Start()
        {
            frameBufferTimer = new System.Threading.Timer(frameBuffer_tick);
            frameBufferTimer.Change(0, 250);

            processorThread = new Thread(new ThreadStart(processingLoop));
            processorThread.Start();
        }

        /// <summary>
        /// Aborts the main video processing loop
        /// </summary>
        public void Stop()
        {
            frameBufferTimer.Change(0, 0);
            frameBufferTimer = null;
            processorThread?.Abort();
            processorThread = null;
            cap.Dispose();
            cap = null;
        }


        /// <summary>
        /// Grabs a frame for the frame buffer every tick.
        /// </summary>
        public void frameBuffer_tick(object sender)
        {
            if (frameSize == null || cap == null) return;

            Image<Bgr, byte> frame = new Image<Bgr, byte>(frameSize);
            cap.Retrieve(frame);

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


        /// <summary>
        /// Main video processing loop
        /// </summary>
        public void processingLoop()
        {
            cap = new VideoCapture(deviceId);
            cap.SetCaptureProperty(CapProp.FrameHeight, resolution.Height);
            cap.SetCaptureProperty(CapProp.FrameWidth, resolution.Width);

            //Capture a first frame to get the basic information so we can allocate the data array
            Mat tmp = new Mat();
            cap.Retrieve(tmp);
            frameSize = new Size(tmp.Width, tmp.Height);
            int channels = tmp.NumberOfChannels;

            if (tmp.Bitmap == null)
            {
                throw new Exception("Failed to get image from video device");
            }
            
            log.Debug(String.Format("Found resolution: {0}x{1} with {2} channels.", tmp.Width, tmp.Height, tmp.NumberOfChannels));
            if(channels != 3)
            {
                throw new Exception(String.Format("Unexcepted channel count: {0}", tmp.NumberOfChannels));
            }


            //Initalize the currentFrame mat with a byte[] pointer so we can access its data directly without a conversion to Image<>
            byte[] data = new byte[frameSize.Width * frameSize.Height * 3];
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Mat currentFrame = new Mat(frameSize, DepthType.Cv8U, 3, dataHandle.AddrOfPinnedObject(), frameSize.Width * 3);

            bool WasBlack = false;
            bool WasClear = false;
            int skip = frameSize.Width / 50;
            while (true)
            {
                cap.Retrieve(currentFrame);
                Dictionary<int, int> hues = getHues(data, frameSize, skip);

                if(WasBlack)
                {
                    WasBlack = (getColorCoverage(hues, 0) > 90);
                    continue;
                }

                if(WasClear)
                {
                    int primary = getPrimaryColor(hues);
                    WasClear = (hues[primary] > 95 && Math.Abs(50 - primary) < 10);
                    continue;
                }

                if (getColorCoverage(hues, 0) > 90)
                {
                    WasBlack = true;
                    VideoEventArgs args = new VideoEventArgs();
                    args.frameBuffer = copyFrameBuffer();
                    onBlackScreen(args);
                }
                else
                {
                    int primary = getPrimaryColor(hues);
                    if(hues[primary] > 95 && Math.Abs(50 - primary) < 10) //the SMM clear screen is roughly hue(50)
                    {
                        WasClear = true;
                        VideoEventArgs args = new VideoEventArgs();
                        args.frameBuffer = copyFrameBuffer();
                        onClearScreen(args);
                    }
                    // XXX: Might be useful to know that the clear screen with the timers is 48% Yellow
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
        public int getColorCoverage(Dictionary<int,int> hues, int hue, int fuzzy=0)
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
        public int getPrimaryColor(Dictionary<int, int> hues)
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
        public Dictionary<int, int> getHues(byte[] data, Size frameSize, int skip=20)
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
            Image<Bgr, byte>[] cloned = new Image<Bgr, byte>[frameBufferLength];
            for(int i=0;i<frameBuffer.Length; i++)
            {
                cloned[i] = frameBuffer[i]?.Clone();
            }
            return cloned;

        }

        public event EventHandler<VideoEventArgs> BlackScreen;
        protected virtual void onBlackScreen(VideoEventArgs e)
        {
            BlackScreen?.Invoke(this, e);
            for (int i = 0; i < e.frameBuffer.Length; i++)
            {
                e.frameBuffer[i]?.Dispose();
            }
        }
        public event EventHandler<VideoEventArgs> ClearScreen;
        protected virtual void onClearScreen(VideoEventArgs e)
        {
            ClearScreen?.Invoke(this, e);
            for (int i = 0; i < e.frameBuffer.Length; i++)
            {
                e.frameBuffer[i]?.Dispose();
            }
        }

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
    }
}
