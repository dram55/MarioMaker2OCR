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
        private Size resolution720 = new Size(1280, 720);
        private Size resolution480 = new Size(640, 480);
        private Rectangle levelCodeArea;
        private Rectangle creatorNameArea;
        private Rectangle levelTitleArea;
        private Rectangle levelCodeArea720p = new Rectangle(78, 175, 191, 33); // based on 1280x720
        private Rectangle creatorNameArea720 = new Rectangle(641, 173, 422, 39); // based on 1280x720
        private Rectangle levelTitleArea720 = new Rectangle(100, 92, 1080, 43); // based on 1280x720

        private Mat levelSelectScreen;
        private readonly Mat levelSelectScreen720 = new Image<Bgr, byte>("referenceImage.jpg").Mat; // based on 1280x720

        private readonly Image<Gray, byte> tmplDeathBig = new Image<Gray, byte>("./templates/480/death_big.png");         // Primary death bubble, larger than past death bubbles
        private readonly Image<Gray, byte> tmplDeathPartial = new Image<Gray, byte>("./templates/480/death_partial.png"); // In area with a lot of deaths the bubble may be partially obscured
        private readonly Image<Gray, byte> tmplDeathSmall = new Image<Gray, byte>("./templates/480/death_small.png");     // Death bubbles caused by past deaths/other players
        private readonly Image<Gray, byte> tmplExit = new Image<Gray, byte>("./templates/480/exit.png");
        private readonly Image<Gray, byte> tmplRestart = new Image<Gray, byte>("./templates/480/startover.png");
        private readonly Image<Gray, byte> tmplQuit = new Image<Gray, byte>("./templates/480/quit.png");

        public DsDevice SelectedDevice => (deviceComboBox.SelectedItem as dynamic)?.Value;
        public Size SelectedResolution => (resolutionsCombobox.SelectedItem as dynamic)?.Value;

        private FormPreview previewer = new FormPreview();
        private VideoProcessor processor;

        private string jsonOutputFolder = "";

        public Form1()
        {
            InitializeComponent();

            jsonOutputFolder = Properties.Settings.Default.OutputFolder;
            numPort.Value = Properties.Settings.Default.SelectedPort;

            initializeToolTips();
            loadVideoDevices();
            loadResolutions();
        }

        private void initializeToolTips()
        {
            new ToolTip().SetToolTip(ocrLabel, "Last level information read in.");
            new ToolTip().SetToolTip(deviceLabel, "Available capture devices.");
            new ToolTip().SetToolTip(propertiesButton, "Properties for the selected capture device.");
            new ToolTip().SetToolTip(numPortLabel, "Port to run the Web Server on (used for OBS Browser Source)");
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

        private void loadResolutions()
        {
            resolutionsCombobox.DisplayMember = "Name";
            resolutionsCombobox.ValueMember = "Value";


            resolutionsCombobox.Items.Add(new { Name = "640x480", Value = new Size(640, 480) });
            resolutionsCombobox.Items.Add(new { Name = "1280x720", Value = new Size(1280, 720) });
            resolutionsCombobox.Items.Add(new { Name = "1920x1080", Value = new Size(1920, 1080) });

            resolutionsCombobox.SelectedIndex = Properties.Settings.Default.SelectedResolutionIndex;
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

        private void clearLevelFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Level emptyLevel = new Level();
            writeLevelToFile(emptyLevel);
        }

        private void writeLevelToFile(Level level)
        {
            // Do not save JSON if folder is not selected
            if (string.IsNullOrWhiteSpace(jsonOutputFolder)) return;
            try
            {
                LevelWrapper wrappedLevel = new LevelWrapper() { level = level };
                string json = JsonConvert.SerializeObject(wrappedLevel);
                File.WriteAllText(Path.Combine(jsonOutputFolder, LEVEL_JSON_FILE), json);
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
                // resize reference image based on current resolution
                levelSelectScreen = ImageLibrary.ChangeSize(levelSelectScreen720, resolution720, SelectedResolution);

                // resize rectangles based on current resolution
                levelCodeArea = ImageLibrary.ChangeSize(levelCodeArea720p, resolution720, SelectedResolution);
                creatorNameArea = ImageLibrary.ChangeSize(creatorNameArea720, resolution720, SelectedResolution);
                levelTitleArea = ImageLibrary.ChangeSize(levelTitleArea720, resolution720, SelectedResolution);

                SMMServer.port = Decimal.ToUInt16(numPort.Value);
                log.Info(String.Format("Start Web Server on http://localhost:{0}/", SMMServer.port));
                SMMServer.Start();

                processor = new VideoProcessor(deviceComboBox.SelectedIndex, SelectedResolution);
                processor.BlackScreen += VideoProcessor_BlackScreen;
                processor.ClearScreen += VideoProcessor_ClearScreen;
                processor.NewFrame += VideoProcessor_NewFrame;
                processor.Start();
                lockForm();
            }
            catch (Exception ex)
            {
                processException("Error starting video device", ex);
            }
        }

        private void VideoProcessor_NewFrame(object sender, VideoProcessor.VideoEventArgs e)
        {
            previewer.SetLiveFrame(e.currentFrame);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(() => unlockForm()));
            SMMServer.Stop();
            processor?.Stop();
            processor = null;
        }

        private void lockForm()
        {
            deviceComboBox.Enabled = false;
            resolutionsCombobox.Enabled = false;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            propertiesButton.Enabled = false;
            ocrTextBox.Text = "";
            numPort.Enabled = false;
            processingLabel.Text = "";
            processingLabel.Visible = true;
            processStatusIcon.BackColor = Color.Green;
            webServerAddressStatusLabel.Text = $"http://localhost:{numPort.Value}";
        }

        private void unlockForm()
        {
            processStatusIcon.BackColor = Color.Red;
            deviceComboBox.Enabled = true;
            resolutionsCombobox.Enabled = true;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            processingLabel.Visible = false;
            numPort.Enabled = true;
            webServerAddressStatusLabel.Text = $"";
        }

        /// <summary>
        /// Event callback for the Clear Screen event generatead by the VideoProcessor
        /// </summary>
        private void VideoProcessor_ClearScreen(object sender, VideoProcessor.VideoEventArgs e)
        {
            log.Debug("Detected Level Clear");
            SMMServer.BroadcastEvent("clear");
        }

        /// <summary>
        /// Event Callback for the Black Screen event generated by the VideoProcessor
        /// </summary>
        private void VideoProcessor_BlackScreen(object sender, VideoProcessor.VideoEventArgs e)
        {
            log.Debug("Detected a black screen");
            BeginInvoke((MethodInvoker)(() => processingLabel.Text = "Processing black screen..."));
            Image<Bgr, byte> frame = e.frameBuffer[e.frameBuffer.Length - 5];
            double imageMatchPercent = ImageLibrary.CompareImages(frame, levelSelectScreen);
            if(imageMatchPercent > 0.94)
            {
                log.Info("Detected new level.");
                previewer.SetLastMatch(frame, new Rectangle[] { levelCodeArea, creatorNameArea, levelTitleArea });
                BeginInvoke((MethodInvoker)(() => processingLabel.Text = "Processing level screen..."));
                Level level = getLevelFromCurrentFrame(frame);
                writeLevelToFile(level);
                BeginInvoke((MethodInvoker)(() => ocrTextBox.Text = level.code + "  |  " + level.author + "  |  " + level.name));
                BeginInvoke((MethodInvoker)(() => processingLabel.Text = ""));
                SMMServer.BroadcastLevel(level);
            }
            else
            {
                // Not a new level, see if we can detect a template.
                BeginInvoke((MethodInvoker)(() => processingLabel.Text = "Processing events..."));
                Dictionary<String, bool> events = new Dictionary<String, bool>();
                events.Add("death", false);
                events.Add("restart", false);
                events.Add("exit", false);
                Size frameSize = new Size(e.frameBuffer[0].Width, e.frameBuffer[0].Height);

                foreach (Image<Bgr, byte> f in e.frameBuffer)
                {
                    Image<Gray, byte> grayscaleFrame = f.Mat.ToImage<Gray, byte>().Resize(640,480, Inter.Cubic);
                    Image<Gray, byte>[] deathTemplates = new Image<Gray, byte>[] { tmplDeathBig, tmplDeathSmall, tmplDeathPartial };

                    //grayscaleFrame.Save("frame_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png"); // XXX: Useful for debugging template false-negatives, and for getting templates

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
                                boundaries.Add(ImageLibrary.ChangeSize(new Rectangle(loc.Value.X - tmpl.Width, loc.Value.Y - tmpl.Height, tmpl.Width * 3, tmpl.Height * 3), resolution480, frameSize));
                            }
                        }
                        if (events["death"])
                        {
                            previewer.SetLastMatch(f, boundaries.ToArray());
                        }
                    }

                    //Just in case a user hoved one then changed to the other button, only the last one counts, so don't look after one has been found.
                    if (!events["restart"] && !events["exit"])
                    {
                        Point? loc = ImageLibrary.IsTemplatePresent(grayscaleFrame, tmplRestart, 0.8);
                        if (loc.HasValue)
                        {
                            events["restart"] = true;
                            Rectangle match = ImageLibrary.ChangeSize(new Rectangle(loc.Value.X, loc.Value.Y, tmplRestart.Width, tmplRestart.Height), resolution480, frameSize);
                            previewer.SetLastMatch(f, new Rectangle[] { match });
                        }
                        else
                        {
                            // Really shouldn't happen but popped this in `else` just to prevent one frame from detecting both.
                            loc = ImageLibrary.IsTemplatePresent(grayscaleFrame, tmplExit, 0.7);
                            if (loc.HasValue)
                            {
                                events["exit"] = true;
                                Rectangle match = ImageLibrary.ChangeSize(new Rectangle(loc.Value.X, loc.Value.Y, tmplExit.Width, tmplExit.Height), resolution480, frameSize);
                                previewer.SetLastMatch(f, new Rectangle[] { match });
                            }

                            //Quit button in Endless runs instead of exit.
                            loc = ImageLibrary.IsTemplatePresent(grayscaleFrame, tmplQuit, 0.9);
                            if (loc.HasValue)
                            {
                                events["exit"] = true;
                                Rectangle match = ImageLibrary.ChangeSize(new Rectangle(loc.Value.X, loc.Value.Y, tmplQuit.Width, tmplQuit.Height), resolution480, frameSize);
                                previewer.SetLastMatch(f, new Rectangle[] { match });
                            }
                        }
                    }
                }

                BeginInvoke((MethodInvoker)(() => processingLabel.Text = ""));
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

        }

        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            processor?.Stop();
            Properties.Settings.Default.OutputFolder = jsonOutputFolder;
            Properties.Settings.Default.SelectedDevice = (deviceComboBox.SelectedItem as dynamic)?.Name;
            Properties.Settings.Default.SelectedPort = Decimal.ToInt32(numPort.Value);
            Properties.Settings.Default.SelectedResolutionIndex = resolutionsCombobox.SelectedIndex;
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

        private void jSONFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a location to save the JSON output file.";

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                jsonOutputFolder = dialog.SelectedPath;
            }
        }

        private void numPort_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SelectedPort = Decimal.ToInt32(numPort.Value);
            Properties.Settings.Default.Save();
        }

        private void resolutionsCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SelectedResolutionIndex = resolutionsCombobox.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SelectedDevice = (deviceComboBox.SelectedItem as dynamic)?.Name ?? "";
            Properties.Settings.Default.Save();
        }
    }
}
