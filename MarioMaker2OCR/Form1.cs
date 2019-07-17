using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using Newtonsoft.Json;
using MarioMaker2OCR.Objects;
using DirectShowLib;

namespace MarioMaker2OCR
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string LEVEL_JSON_FILE = "ocrLevel.json";
        private VideoCapture videoDevice;
        private Size resolution720 = new Size(1280, 720);
        private readonly System.Timers.Timer processVideoFrameTimer;
        private Rectangle levelCodeArea;
        private Rectangle creatorNameArea;
        private Rectangle levelTitleArea;
        private Rectangle levelCodeArea720p = new Rectangle(81, 178, 190, 25); // based on 1280x720
        private Rectangle creatorNameArea720 = new Rectangle(641, 173, 422, 39); // based on 1280x720
        private Rectangle levelTitleArea720 = new Rectangle(100, 92, 1080, 43); // based on 1280x720

        private Mat levelSelectScreen;
        private readonly Mat levelSelectScreen720 = new Image<Bgr, byte>("referenceImage.jpg").Mat; // based on 1280x720

        private Image<Gray, byte> tmplDeathBig;
        private Image<Gray, byte> tmplDeathPartial;
        private Image<Gray, byte> tmplDeathSmall;
        private Image<Gray, byte> tmplRestart;
        private Image<Gray, byte> tmplExit;
        private readonly Image<Gray, byte> tmplDeathBig1080 = new Image<Gray, byte>("./templates/death_big.png");         // Primary death bubble, larger than past death bubbles
        private readonly Image<Gray, byte> tmplDeathPartial1080 = new Image<Gray, byte>("./templates/death_partial.png"); // In area with a lot of deaths the bubble may be partially obscured
        private readonly Image<Gray, byte> tmplDeathSmall1080 = new Image<Gray, byte>("./templates/death_small.png");     // Death bubbles caused by past deaths/other players
        private readonly Image<Gray, byte> tmplRestart1080 = new Image<Gray, byte>("./templates/startover.png");
        private readonly Image<Gray, byte> tmplExit1080 = new Image<Gray, byte>("./templates/exit.png");

        public Size SelectedResolution => (resolutionsCombobox.SelectedItem as dynamic)?.Value;
        public DsDevice SelectedDevice => (deviceComboBox.SelectedItem as dynamic)?.Value;

        private FormPreview previewer = new FormPreview();

        // Flags so we only run once every time a key frame is detected
        private bool WasBlack = false;
        private bool WasClear = false;

        // Simple fixed-size buffer that holds the most recent frames 0 is the current frame.
        private Mat[] FrameBuffer = { null, null, null, null, null, null, null, null, null, null };

        public Form1()
        {
            InitializeComponent();

            outputFolderTextbox.Text = Properties.Settings.Default.OutputFolder;
            numPort.Value = Properties.Settings.Default.SelectedPort;

            processVideoFrameTimer = new System.Timers.Timer(250);
            processVideoFrameTimer.Elapsed += readScreenTimer_Tick;

            initializeToolTips();
            loadVideoDevices();
            loadResolutions();
        }

        private void initializeToolTips()
        {
            new ToolTip().SetToolTip(ocrLabel, "Last level information read in.");
            new ToolTip().SetToolTip(outputFolderLabel, $"Folder to save {LEVEL_JSON_FILE}");
            new ToolTip().SetToolTip(deviceLabel, "Available capture devices.");
            new ToolTip().SetToolTip(propertiesButton, "Properties for the selected capture device.");
            new ToolTip().SetToolTip(numPortLabel, "Port to run the Web Server on (used for OBS Browser Source)");
        }

        private void loadResolutions()
        {
            resolutionsCombobox.DisplayMember = "Name";
            resolutionsCombobox.ValueMember = "Value";

            addResolutionToCombobox(new Size(1280, 720));
            addResolutionToCombobox(new Size(1920, 1080));

            resolutionsCombobox.SelectedIndex = Properties.Settings.Default.SelectedResolutionIndex; // default
        }

        private void addResolutionToCombobox(Size res)
        {
            resolutionsCombobox.Items.Add(new { Name = $"{res.Width} x{res.Height}", Value = res });
        }

        private void loadVideoDevices()
        {
            deviceComboBox.DisplayMember = "Name";
            deviceComboBox.ValueMember = "Value";

            List<DsDevice> videoDevices = DirectShowLibrary.GetCaptureDevices();

            for (int i = 0; i < videoDevices.Count; i++)
            {
                deviceComboBox.Items.Add(new { videoDevices[i].Name, Value = videoDevices[i] });

                // load default
                if (Properties.Settings.Default.SelectedDevice == videoDevices[i].Name)
                    deviceComboBox.SelectedIndex = i;
            }
        }

        /// <summary>
        /// Add a frame to the FrameBuffer, acts like a very limited Ring Buffer of sorts.
        /// Frames are Disposed once they leave the buffer.
        /// </summary>
        /// <param name="frame">Frame to be added to the buffer</param>
        private void addFrameToBuffer(Mat frame)
        {
            (FrameBuffer[FrameBuffer.Length - 1])?.Dispose();
            for (int i = FrameBuffer.Length-1; i > 0; i--)
            {
                FrameBuffer[i] = FrameBuffer[i - 1];
            }
            FrameBuffer[0] = frame;
        }

        private void processVideoFrame()
        {
            Mat currentFrame = new Mat();
            try
            {
                if (videoDevice==null) return;
                videoDevice.Retrieve(currentFrame);
                if (currentFrame.Bitmap == null)
                {
                    throw new Exception("Unable to retrieve the current video frame. Device could be in use by another program.");
                }

                addFrameToBuffer(currentFrame); //frame will be automatically disposed when it leaves the FrameBuffer.
                previewer.SetLiveFrame(FrameBuffer[0]);

                Image<Bgr, byte> imgFrame = currentFrame.ToImage<Bgr, byte>();

                // Scan for a solid color screen, likely Black
                if(ImageLibrary.IsRegionSolid(imgFrame, new Rectangle(0, 0, imgFrame.Width, imgFrame.Height))) {
                    WasClear = false;
                    Bgr color = imgFrame[0, 0];
                    if (color.Red <= 20 && color.Green <= 20 && color.Blue <= 20) //Black
                    {
                        if (!WasBlack)
                        {
                            WasBlack = true;
                            onBlackScreen();
                        }
                    }
                }
                else
                {
                    WasBlack = false;
                    // Scan just the bottom fifth of the frame, if its solid but the entire frame isn't, its likely the clear screen.
                    Rectangle clearRegion = new Rectangle(0, (imgFrame.Height / 5) * 4, imgFrame.Width, imgFrame.Height / 5);
                    if (ImageLibrary.IsRegionSolid(imgFrame, clearRegion))
                    {
                        Bgr color = imgFrame[0, 0];
                        if (color.Red > 220 && color.Green > 200 && color.Blue < 30) //Yellow-ish
                        {
                            if(!WasClear)
                            {
                                WasClear = true;
                                onClearScreen();
                            }
                        }
                    }
                    else
                    {
                        WasClear = false;
                    }
                }
            }
            catch (Exception ex)
            {
                processException("Error Processing Video Frame", ex);
            }
        }

        /// <summary>
        /// Handler that is called when a Black Screen is detected
        /// </summary>
        private void onBlackScreen()
        {
            log.Debug("Detected Black Screen");

            // Check if this is the start of a new level
            Mat currentFrame = FrameBuffer[FrameBuffer.Length - 5];
            double imageMatchPercent = ImageLibrary.CompareImages(currentFrame, levelSelectScreen);
            if (imageMatchPercent > .94)
            {
                log.Info("Detected new level.");

                BeginInvoke((MethodInvoker)(() => processingLevelLabel.Visible = true));
                Level level = getLevelFromCurrentFrame(currentFrame.ToImage<Bgr, byte>());
                writeLevelToFile(level);
                BeginInvoke((MethodInvoker)(() => ocrTextBox.Text = level.code + "  |  " + level.author + "  |  " + level.name));
                previewer.SetLastMatch(currentFrame.Clone(), new Rectangle[] { levelCodeArea, creatorNameArea, levelTitleArea });
                BeginInvoke((MethodInvoker)(() => processingLevelLabel.Visible = false));

                SMMServer.BroadcastLevel(level);
            }
            else
            {
                // Not a new level, see if we can detect a template.
                Dictionary<String, bool> events = new Dictionary<String, bool>();
                events.Add("death", false);
                events.Add("restart", false);
                events.Add("exit", false);

                foreach (Mat f in FrameBuffer)
                {
                    Image<Gray, byte> grayscaleFrame = f.ToImage<Gray, byte>();
                    Image<Gray, byte>[] deathTemplates = new Image<Gray, byte>[] { tmplDeathBig, tmplDeathSmall, tmplDeathPartial };

                    //grayscaleFrame.Save(DateTime.Now.ToString("frame_yyyyMMddHHmmssffff") + ".png"); // XXX: Useful for debugging template false-negatives, and for getting templates

                    //Once we have found a death, don't scan the rest of the frames for one
                    if (!events["death"])
                    {
                        List<Rectangle> boundaries = new List<Rectangle>();
                        foreach (Image<Gray, byte> tmpl in deathTemplates)
                        {
                            // FIXME: Better solution would be to map templates to their settings (threshold, fn, etc.) and their triggered event rather than doing this special case here.
                            double thresh = 0.8;
                            if (tmpl == tmplDeathPartial) thresh = 0.9;

                            Point? loc = ImageLibrary.IsTemplatePresent(grayscaleFrame, tmpl, thresh);
                            if (loc.HasValue)
                            {
                                events["death"] = true;
                                boundaries.Add(new Rectangle(loc.Value.X - tmpl.Width, loc.Value.Y - tmpl.Height, tmpl.Width * 3, tmpl.Height * 3));
                            }
                        }
                        if(events["death"])
                        {
                            previewer.SetLastMatch(f.Clone(), boundaries.ToArray());
                        }
                    }

                    //Just in case a user hoved one then changed to the other button, only the last one counts, so don't look after one has been found.
                    if(!events["restart"] && !events["exit"])
                    {
                        Point? loc = ImageLibrary.IsTemplatePresent(grayscaleFrame, tmplRestart, 0.8);
                        if (loc.HasValue)
                        {
                            events["restart"] = true;
                            previewer.SetLastMatch(f.Clone(), new Rectangle[] { new Rectangle(loc.Value.X, loc.Value.Y, tmplRestart.Width, tmplRestart.Height) });
                        }
                        else
                        {
                            // Really shouldn't happen but popped this in `else` just to prevent one frame from detecting both.
                            loc = ImageLibrary.IsTemplatePresent(grayscaleFrame, tmplExit, 0.7);
                            if (loc.HasValue)
                            {
                                events["exit"] = true;
                                previewer.SetLastMatch(f.Clone(), new Rectangle[] { new Rectangle(loc.Value.X, loc.Value.Y, tmplExit.Width, tmplExit.Height) });
                            }
                        }
                    }
                }

                if (events["death"])
                {
                    log.Info("Detected death");
                    SMMServer.BroadcastEvent("death");
                }
                if (events["restart"])
                {
                    log.Info("Detected restart");
                    SMMServer.BroadcastEvent("restart");
                }
                if (events["exit"])
                {
                    log.Info("Detected exit");
                    SMMServer.BroadcastEvent("exit");
                }
            }
        }

        /// <summary>
        /// Handler that is called when a "Course Clear" screen is detected
        /// </summary>
        private void onClearScreen()
        {
            log.Info("Detected Level Clear");
            SMMServer.BroadcastEvent("clear");
        }

        private Level getLevelFromCurrentFrame(Image<Bgr, byte> currentFrame)
        {
            try
            {
                Level ocrLevel = new Level();

                // Level Code
                currentFrame.ROI = levelCodeArea;
                ocrLevel.code = getStringFromLevelCodeImage(currentFrame);

                // Level Title
                currentFrame.ROI = levelTitleArea;
                ocrLevel.name = getStringFromImage(currentFrame);

                // Creator Name
                currentFrame.ROI = creatorNameArea;
                ocrLevel.author = getStringFromImage(currentFrame);

                return ocrLevel;
            }
            catch(Exception ex)
            {
                processException("Error Performing OCR", ex);
            }

            return null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static string getStringFromImage(Image<Bgr, byte> image)
        {
            Image<Gray, byte> ocrReadyImage = ImageLibrary.PrepareImageForOCR(image);
            return OCRLibrary.GetStringFromImage(ocrReadyImage);
        }

        private static string getStringFromLevelCodeImage(Image<Bgr, byte> image)
        {
            Image<Gray, byte> ocrReadyImage = ImageLibrary.PrepareImageForOCR(image);
            return OCRLibrary.GetStringFromLevelCodeImage(ocrReadyImage);
        }

        private static readonly object lockObject = new object();
        private void readScreenTimer_Tick(object o, EventArgs e)
        {
            var hasLock = false;

            try
            {
                // If process is not locked process frame, else skip.
                Monitor.TryEnter(lockObject, ref hasLock);
                if (!hasLock) return;

                processVideoFrame();
            }
            finally
            {
                if (hasLock)
                {
                    Monitor.Exit(lockObject);
                }
            }
        }

        private void clearLevelFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Level emptyLevel = new Level();
            writeLevelToFile(emptyLevel);
        }

        private void writeLevelToFile(Level level)
        {
            try
            {
                LevelWrapper wrappedLevel = new LevelWrapper() { level = level };
                string json = JsonConvert.SerializeObject(wrappedLevel);
                File.WriteAllText(Path.Combine(outputFolderTextbox.Text, LEVEL_JSON_FILE), json);
            }
            catch (Exception ex)
            {
                processException("Error writing to json file", ex);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (deviceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a video device first.");
                return;
            }
            try
            {
                // Set Capture Card Resolution
                initializeAndStartVideoDevice();

                // resize reference image based on current resolution
                levelSelectScreen = ImageLibrary.ChangeSize(levelSelectScreen720, resolution720, SelectedResolution);

                // resize rectangles based on current resolution
                levelCodeArea = ImageLibrary.ChangeSize(levelCodeArea720p, resolution720, SelectedResolution);
                creatorNameArea = ImageLibrary.ChangeSize(creatorNameArea720, resolution720, SelectedResolution);
                levelTitleArea = ImageLibrary.ChangeSize(levelTitleArea720, resolution720, SelectedResolution);

                // resize templates based on current resolution
                if(SelectedResolution.Height != 1080)
                {
                    log.Info("Resizing templates");
                    tmplDeathBig = tmplDeathBig1080.Resize(0.6666, Emgu.CV.CvEnum.Inter.Nearest);
                    tmplDeathPartial = tmplDeathPartial1080.Resize(0.6666, Emgu.CV.CvEnum.Inter.Nearest);
                    tmplDeathSmall = tmplDeathSmall1080.Resize(0.6666, Emgu.CV.CvEnum.Inter.Nearest);
                    tmplRestart = tmplRestart1080.Resize(0.6666, Emgu.CV.CvEnum.Inter.Nearest);
                    tmplExit = tmplExit1080.Resize(0.6666, Emgu.CV.CvEnum.Inter.Nearest);
                }
                else
                {
                    tmplDeathBig = tmplDeathBig1080;
                    tmplDeathPartial = tmplDeathPartial1080;
                    tmplDeathSmall = tmplDeathSmall1080;
                    tmplRestart = tmplRestart1080;
                    tmplExit = tmplExit1080;
                }

                SMMServer.port = Decimal.ToUInt16(numPort.Value);
                log.Info(String.Format("Start Web Server on http://localhost:{0}/", SMMServer.port));
                SMMServer.Start();

                log.Info($"Connecting to {deviceComboBox.SelectedIndex} - {deviceComboBox.SelectedItem}");
                log.Info($"Using Resolution: {videoDevice.Width}x{videoDevice.Height}");
                lockForm();
            }
            catch (Exception ex)
            {
                processException("Error starting video device", ex);
                throw ex;
            }
        }

        private void initializeAndStartVideoDevice()
        {
            videoDevice = new VideoCapture(deviceComboBox.SelectedIndex);
            videoDevice.SetCaptureProperty(CapProp.FrameHeight, SelectedResolution.Height);
            videoDevice.SetCaptureProperty(CapProp.FrameWidth, SelectedResolution.Width);
            videoDevice.Start();
        }

        private void lockForm()
        {
            processVideoFrameTimer.Start();
            deviceComboBox.Enabled = false;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            resolutionsCombobox.Enabled = false;
            propertiesButton.Enabled = false;
            ocrTextBox.Text = "";
            numPort.Enabled = false;
            processStatusIcon.BackColor = Color.Green;
            webServerAddressStatusLabel.Text = $"http://localhost:{numPort.Value}";
        }

        private void unlockForm()
        {
            processVideoFrameTimer.Stop();
            processStatusIcon.BackColor = Color.Red;
            deviceComboBox.Enabled = true;
            resolutionsCombobox.Enabled = true;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            processingLevelLabel.Visible = false;
            numPort.Enabled = true;
            webServerAddressStatusLabel.Text = $"";
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            videoDevice.Stop();
            videoDevice.Dispose();
            BeginInvoke(new MethodInvoker(() => unlockForm()));
            SMMServer.Stop();
        }

        private void propertiesButton_Click(object sender, EventArgs e)
        {
            if (deviceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a video device first.");
                return;
            }
            try
            {
                DirectShowLibrary.DisplayPropertyPage(SelectedDevice.Mon, this.Handle);
            }
            catch (Exception ex)
            {
                processException("Error displaying device properties", ex);
            }
        }

        private void processException(string caption, Exception ex)
        {
            log.Error($"{caption}: {ex.Message}");
            log.Debug(ex.StackTrace);

            stopButton_Click(null, null);
            MessageBox.Show(ex.Message, "Error Processing Video", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void selectFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a location to save the output file.";

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                outputFolderTextbox.Text = dialog.SelectedPath;
            }
        }

        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.OutputFolder = outputFolderTextbox.Text;
            Properties.Settings.Default.SelectedDevice = (deviceComboBox.SelectedItem as dynamic)?.Name;
            Properties.Settings.Default.SelectedResolutionIndex = resolutionsCombobox.SelectedIndex;
            Properties.Settings.Default.SelectedPort = Decimal.ToInt32(numPort.Value);
            Properties.Settings.Default.Save();
        }

        private void showPreviewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (previewer.IsDisposed)
            {
                previewer = new FormPreview();
            }
            previewer.Show();
            previewer.BringToFront();
        }

        private void webServerAddressStatusLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"http://localhost:{numPort.Value}");
        }
    }
}
