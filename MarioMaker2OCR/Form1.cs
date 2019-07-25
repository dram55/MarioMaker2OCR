using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

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

        private Mat levelDetailScreen;
        private readonly Mat levelSelectScreen720 = new Image<Bgr, byte>("referenceImage.jpg").Mat; // based on 1280x720

        private readonly EventTemplate[] templates = new EventTemplate[] {
            new EventTemplate("./templates/480/death_big.png", "death", 0.8),
            new EventTemplate("./templates/480/death_small.png", "death", 0.8),
            new EventTemplate("./templates/480/death_partial.png", "death", 0.9),
            new EventTemplate("./templates/480/exit.png", "exit", 0.8),
            new EventTemplate("./templates/480/quit.png", "exit", 0.9),
            new EventTemplate("./templates/480/startover.png", "restart", 0.8),
        };

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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                levelDetailScreen = ImageLibrary.ChangeSize(levelSelectScreen720, resolution720, SelectedResolution);

                SMMServer.port = decimal.ToUInt16(numPort.Value);
                log.Info(string.Format("Start Web Server on http://localhost:{0}/", SMMServer.port));
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
            SMMServer.Stop();
            if(processor != null)
            {
                processor.Stop();
                processor.Dispose();
                processor = null;
            }
            BeginInvoke(new MethodInvoker(() => unlockForm()));
        }

        private void clearLevelFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Level emptyLevel = new Level();
            writeLevelToFile(emptyLevel);
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

            // Is this a new level?
            Image<Bgr, byte> frame = e.frameBuffer[e.frameBuffer.Length - 5];
            double imageMatchPercent = ImageLibrary.CompareImages(frame, levelDetailScreen);
            if(imageMatchPercent > 0.94)
            {
                log.Info("Detected new level.");

                BeginInvoke((MethodInvoker)(() => processingLabel.Text = "Processing level screen..."));

                Level level = OCRLibrary.GetLevelFromFrame(frame);
                writeLevelToFile(level);
                SMMServer.BroadcastLevel(level);

                BeginInvoke((MethodInvoker)(() => ocrTextBox.Text = level.code + "  |  " + level.author + "  |  " + level.name));
                BeginInvoke((MethodInvoker)(() => processingLabel.Text = ""));

                previewer.SetLastMatch(frame);
            }
            else
            {
                // Not a new level, see if we can detect a template.
                Dictionary<String, bool> events = new Dictionary<String, bool>
                {
                    { "death", false },
                    { "restart", false },
                    { "exit", false },
                };


                BeginInvoke((MethodInvoker)(() => processingLabel.Text = "Processing events..."));
                foreach (Image<Bgr, byte> f in e.frameBuffer)
                {
                    Image<Gray, byte> grayscaleFrame = f.Mat.ToImage<Gray, byte>().Resize(640, 480, Inter.Cubic);
                    //grayscaleFrame.Save("frame_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png"); // XXX: Useful for debugging template false-negatives, and for getting templates

                    List<Rectangle> boundaries = new List<Rectangle>();
                    foreach (EventTemplate tmpl in templates)
                    {
                        if (events[tmpl.eventType]) continue;
                        Point loc = tmpl.getLocation(grayscaleFrame);
                        if (!loc.IsEmpty) { 
                            events[tmpl.eventType] = true;
                            boundaries.Add(ImageLibrary.ChangeSize(new Rectangle(loc.X, loc.Y, tmpl.template.Width, tmpl.template.Height), grayscaleFrame.Size, f.Size));
                            previewer.SetLastMatch(f, boundaries.ToArray());
                        }
                    }
                }
                BeginInvoke((MethodInvoker)(() => processingLabel.Text = ""));

                foreach (var evt in events)
                {
                    if (evt.Value)
                    {
                        log.Info(String.Format("Detected {0}.", evt.Key));
                        SMMServer.BroadcastEvent(evt.Key);
                    }
                }
            }

            frame.Dispose();
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
