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
using System.Runtime.CompilerServices;
using MarioMaker2OCR.Objects;


namespace MarioMaker2OCR
{
    class VideoProcessor : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool disposed = false; // Flag to indicate the object has been disposed
        public const int NO_DEVICE = -1; // Constant indicating that no video device was used
        public readonly Size TEMPLATE_FRAME_SIZE = new Size(640, 480);
        public Size frameSize;          // Contains the Size() object for the frame, needed for frameBuffer_tick to create the iamge

        private VideoCapture cap;        // EmguCV VideoCapture device object
        private int deviceId;            // Device id of the video capture device
        private Thread processorThread;  // The thread performing the main video processing
        private System.Threading.Timer frameBufferTimer; // Timer used to fill the frame buffer
        private Image<Bgr, byte>[] frameBuffer = new Image<Bgr, byte>[16]; // Frame buffer that is passed to events
        private (bool IsBlack, bool IsClear, bool ShouldStop) flags = (false, false, false); // Contains flags indicating the current status of the video processor

        private const int FRAME_BUFFER_INTERVAL = 250; // put frame in buffer every 250ms

        private Mat levelDetailScreen;
        private string lastEvent;

        private Dictionary<string, List<AbstractTemplate>> templates = new Dictionary<string, List<AbstractTemplate>>();


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

            initializeTemplates();
        }

        public VideoProcessor(int device, Size resolution)
        {
            this.deviceId = device;
            this.cap = createCaptureDevice(device, resolution);
            this.frameSize = getCaptureInfo(this.cap).resolution;

            initializeTemplates();
        }

        private void initializeTemplates()
        {
            var originalLevelScreen = new Image<Bgr, byte>("referenceImage.jpg").Mat;
            levelDetailScreen = ImageLibrary.ChangeSize(originalLevelScreen, new Size(originalLevelScreen.Width, originalLevelScreen.Height), frameSize);

            List<AbstractTemplate> clearDetailTemplates = new List<AbstractTemplate>();
            List<AbstractTemplate> blackScreenTemplates = new List<AbstractTemplate>();
            List<AbstractTemplate> postClearTemplates = new List<AbstractTemplate>();

            clearDetailTemplates.Add(
                new EventColorTemplate("worldrecord", 353, .1, new Rectangle[] {
                    new Rectangle(new Point(407,180), new Size(201, 45)),
                    new Rectangle(new Point(407,80), new Size(201, 50))
                })
            );

            clearDetailTemplates.Add(
                new EventColorTemplate("firstclear", 84, .1, new Rectangle[] {
                    new Rectangle(new Point(407,180), new Size(201, 45)),
                    new Rectangle(new Point(407,80), new Size(201, 50))
                })
            );

            //Start Templates that run on black screen immediately following a clear
            postClearTemplates.Add(
                new EventTemplate("./templates/480/exit.png", "exit", 0.8, new Rectangle[] {
                    new Rectangle(new Point(410,225), new Size(215, 160)), //Clear Screen
                })
            );
            postClearTemplates.Add(
                new EventTemplate("./templates/480/quit_full.png", "exit", 0.8, new Rectangle[] {
                    new Rectangle(new Point(195,225), new Size(215, 160)), //Clear Screen
                })
            );
            postClearTemplates.Add(
                new EventTemplate("./templates/480/startover.png", "restart", 0.8, new Rectangle[] {
                    new Rectangle(new Point(195,225), new Size(215, 160)), //Clear Screen
                })
            );

            // Start Black Screen Templates
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/exit.png", "exit", 0.8, new Rectangle[] {
                    new Rectangle(new Point(400,330), new Size(230, 65)), //Pause Menu
                })
            );
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/quit.png", "exit", 0.9, new Rectangle[] {
                    new Rectangle(new Point(408,338), new Size(224, 70)) //Pause Menu
                })
            );
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/startover.png", "restart", 0.8, new Rectangle[] {
                    new Rectangle(new Point(400,275), new Size(230, 65)), //Pause Menu
                })
            );
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/death_big.png", "death", 0.8)
            );
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/death_small.png", "death", 0.8)
            );
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/death_partial.png", "death", 0.9)
            );
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/gameover.png", "gameover", 0.8, new Rectangle[] {
                    new Rectangle(new Point(187,195), new Size(270, 100))
                })
            );
            blackScreenTemplates.Add(
                new EventTemplate("./templates/480/skip.png", "skip", 0.85, new Rectangle[] {
                    new Rectangle(new Point(308,200), new Size(25, 37))
                })
            );

            if (Properties.Settings.Default.DetectMultipleLanguages)
            {
                postClearTemplates.Add(
                    new EventTemplate("./templates/480/lang_neutral/exit_next.png", "exit", 0.9, new Rectangle[] {
                        new Rectangle(new Point(598,323), new Size(30, 60)), // Clear Screen
                        new Rectangle(new Point(598,223), new Size(30, 60))  // Clear Screen (w/ comments)
                    })
                );

                blackScreenTemplates.Add(
                    new EventTemplate("./templates/480/lang_neutral/startover.png", "restart", 0.8, new Rectangle[] {
                        new Rectangle(new Point(397,269), new Size(243, 71)), // Pause Menu
                        //new Rectangle(new Point(195,225), new Size(230, 160)), // This is ROI is "Start Over" or "Quit" depending on gamemode, leave out for now
                    })
                );
                blackScreenTemplates.Add(
                    new EventTemplate("./templates/480/lang_neutral/quit.png", "exit", 0.96, new Rectangle[] {
                        new Rectangle(new Point(537,331), new Size(103, 71)) //Pause Menu
                    })
                );

            }

            templates.Add("clear", clearDetailTemplates);
            templates.Add("postclear", postClearTemplates);
            templates.Add("black", blackScreenTemplates);

             


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

                NewFrameEventArgs args = new NewFrameEventArgs
                {
                    frame = frame.Clone()
                };
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
            video.Read(tmp);

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
                        if (flags.ShouldStop) return;

                        if (deviceId == NO_DEVICE)
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
                                args.seconds = DateTime.Now.Subtract(blackStart).TotalMilliseconds/1000;
                                new Thread(new ParameterizedThreadStart(onBlackScreenEnd)).Start(args);
                            }
                        }
                        else if (flags.IsClear)
                        {
                            flags.IsClear = isClearFrame(hues);
                            if (!flags.IsClear) WaitForClearStats = true;
                        }
                        else if (WaitForClearStats && isClearWithStatsScreen(hues))
                        {
                            log.Info("Detected level clear.");

                            // For NO_DEVICE, manually skip ahead the 500ms equivalent
                            if (deviceId == NO_DEVICE)
                            {
                                double fps = cap.GetCaptureProperty(CapProp.Fps);
                                int framesToSkip = (int)Math.Floor(.500 / (1 / fps));
                                
                                double i = cap.GetCaptureProperty(CapProp.PosFrames);
                                cap.SetCaptureProperty(CapProp.PosFrames, i + framesToSkip);

                                cap.Read(currentFrame);
                                data = currentFrame.GetRawData();
                            }
                            else
                            {
                                // HACK: Apart from taking up more CPU to do a comparision like the Level Select screen this is the best solution imo
                                // Match happens during transition, so 500ms is long enough to get to the screen, but not long enough to exit and miss it.
                                Thread.Sleep(500);
                                cap.Retrieve(currentFrame);
                            }

                            ClearScreenEventArgs args = new ClearScreenEventArgs();
                            args.frame = currentFrame.Clone().ToImage<Bgr, byte>();

                            // Check to see if this is the clear screen with comments on - things are positioned differently.
                            // Top of screen is yellow if comments are on
                            Size topOfScreen = new Size(frameSize.Width, frameSize.Height / 8);
                            Dictionary<int, int> topHues = getHues(data, topOfScreen, skip);

                            if (isMostlyYellow(topHues)) args.commentsEnabled = true;
                            new Thread(new ParameterizedThreadStart(onClearScreen)).Start(args);
                        }
                        else if (isBlackFrame(hues))
                        {
                            flags.IsBlack = true;
                            WaitForClearStats = false; // If we get a black screen and this is true, something weird is going on
                            blackStart = DateTime.Now;
                            new Thread(new ThreadStart(onBlackScreenStart)).Start();
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

            try
            {
                for (int i = 0; i < frameBuffer.Length; i++)
                {
                    cloned[i] = frameBuffer[i]?.Clone();
                }
            }
            catch (Exception ex)
            {
                log.Debug("Exception in copyFrameBuffer()");
                log.Error(ex);
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
        protected virtual void onBlackScreenEnd(object a)
        {
            if (frameBuffer[0] == null) return;

            BlackScreen?.Invoke(this, (BlackScreenEventArgs)a);
            clearFrameBuffer();
        }


        public class TemplateMatchEventArgs: EventArgs
        {
            public Image<Bgr, byte> frame;
            public Point location;
            public AbstractTemplate template;
        }
        public class LevelScreenEventArgs : TemplateMatchEventArgs
        {
            public Level levelInfo;
        }
        public event EventHandler<LevelScreenEventArgs> LevelScreen;
        public event EventHandler<TemplateMatchEventArgs> TemplateMatch;
        public event EventHandler<TemplateMatchEventArgs> Death;
        public event EventHandler<TemplateMatchEventArgs> Exit;
        public event EventHandler<TemplateMatchEventArgs> Restart;
        public event EventHandler<TemplateMatchEventArgs> Skip;
        public event EventHandler<TemplateMatchEventArgs> GameOver;
        public event EventHandler<ClearScreenEventArgs> WorldRecord;
        public event EventHandler<ClearScreenEventArgs> FirstClear;

        protected virtual void onClear()
        {

        }
        protected virtual void onBlackScreenStart()
        {
            var buffer = copyFrameBuffer();
            var levelFrame = getLevelScreenImageFromBuffer(buffer);

            // Do not process anything if the buffer is empty
            if (levelFrame == null) return;

            double levelScreenMatch = ImageLibrary.CompareImages(levelFrame, levelDetailScreen);
            if(levelScreenMatch > 0.90)
            {
                this.lastEvent = "level";
                Level level = OCRLibrary.GetLevelFromFrame(levelFrame);
                log.Info(String.Format("Detected new level: {0}", level.code));
                LevelScreenEventArgs args = new LevelScreenEventArgs {
                    frame = levelFrame,
                    levelInfo = level,
                };
                LevelScreen?.Invoke(this, args);
            } else
            {
                List<AbstractTemplate> ts;
                switch(this.lastEvent)
                {
                    case "clear":
                        ts = templates["postclear"];
                        break;
                    default:
                        ts = templates["black"];
                        break;
                }

                bool matchFound = false;
                foreach (var frame in buffer)
                {
                    if (frame == null) break;
                    Image<Gray, byte> grayscaleFrame = frame.Mat.ToImage<Gray, byte>().Resize(TEMPLATE_FRAME_SIZE.Width, TEMPLATE_FRAME_SIZE.Height, Inter.Cubic);
                    foreach (var tmpl in ts)
                    {
                        var loc = tmpl.getLocation(grayscaleFrame);
                        if (loc != Point.Empty)
                        {
                            this.lastEvent = tmpl.eventType;
                            log.Info(String.Format("Detected {0}", tmpl.eventType));
                            TemplateMatchEventArgs args = new TemplateMatchEventArgs
                            {
                                frame = frame,
                                location = loc,
                                template = tmpl,
                            };
                            TemplateMatch?.Invoke(this, args);

                            switch(tmpl.eventType)
                            {
                                case "death":
                                    Death?.Invoke(this, args);
                                    break;
                                case "exit":
                                    Exit?.Invoke(this, args);
                                    break;
                                case "restart":
                                    Restart?.Invoke(this, args);
                                    break;
                                case "skip":
                                    Skip?.Invoke(this, args);
                                    break;
                                case "gameover":
                                    GameOver?.Invoke(this, args);
                                    break;
                                default:
                                    log.Error(String.Format("No handler for event type: {0}", tmpl.eventType));
                                    break;
                            }
                            matchFound = true;
                            break;
                        }
                    }
                    if (matchFound) break;
                }
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i]?.Dispose();
                buffer[i] = null;
            }
        }



        /// <summary>
        /// Event fires off whenever a clear screen is detected
        /// </summary>
        public event EventHandler<ClearScreenEventArgs> ClearScreen;
        protected virtual void onClearScreen(object a)
        {
            ClearScreenEventArgs e = (ClearScreenEventArgs)a;
            this.lastEvent = "clear";
            if (frameBuffer[0] == null) return;

            e.clearTime = OCRLibrary.GetClearTimeFromFrame(e.frame, e.commentsEnabled);

            Image<Bgr, byte> resizedFrame = e.frame.Mat.ToImage<Bgr, byte>().Resize(640, 480, Inter.Cubic);
            foreach (var tmpl in templates["clear"])
            {
                var loc = tmpl.getLocation(resizedFrame);
                if (loc != Point.Empty)
                {
                    log.Info(String.Format("Detected {0}", tmpl.eventType));
                    TemplateMatchEventArgs args = new TemplateMatchEventArgs
                    {
                        frame = e.frame,
                        location = loc,
                        template = tmpl,
                    };
                    TemplateMatch?.Invoke(this, args);

                    switch (tmpl.eventType)
                    {
                        case "firstclear":
                            e.firstClear = true;
                            FirstClear?.Invoke(this, e);
                            break; //or count a firstClear as a WR?
                        case "worldrecord":
                            e.worldRecord = true;
                            WorldRecord?.Invoke(this, e);
                            break;
                        default:
                            log.Error(String.Format("No handler for event type: {0}", tmpl.eventType));
                            break;
                    }
                    break;
                }
            }
            ClearScreen?.Invoke(this, e);
            clearFrameBuffer();
        }

        /// <summary>
        /// Fires off every time the frameBuffer adds a new frame.
        /// </summary>
        public event EventHandler<NewFrameEventArgs> NewFrame;
        protected virtual void onNewFrame(NewFrameEventArgs e)
        {
            NewFrame?.Invoke(this, e);
            e.frame.Dispose();
        }
        public class NewFrameEventArgs : EventArgs
        {
            public Image<Bgr, byte> frame;
        }

        public class BlackScreenEventArgs: EventArgs
        {
            public double seconds;
        }

        public class ClearScreenEventArgs : EventArgs
        {
            public Image<Bgr, byte> frame;
            public bool commentsEnabled;
            public string clearTime;
            public bool firstClear;
            public bool worldRecord;
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
