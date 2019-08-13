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
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;



namespace MarioMaker2OCR
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string LEVEL_JSON_FILE = "ocrLevel.json";

        // Product version
        public string CurrentVersion
        {
            get
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return fileVersionInfo.ProductVersion;
            }
        }

        public DsDevice SelectedDevice => (deviceComboBox.SelectedItem as dynamic)?.Value;
        public Size SelectedResolution => (resolutionsCombobox.SelectedItem as dynamic)?.Value;

        private FormPreview previewer = new FormPreview();
        private FormWarpWorld warpworldForm = new FormWarpWorld();
        private WarpWorldAPI WarpWorld;
        private VideoProcessor processor;

        private string jsonOutputFolder = "";

        public Form1()
        {
            log.Info($"Start Application - version: {CurrentVersion}");

            InitializeComponent();

            jsonOutputFolder = Properties.Settings.Default.OutputFolder;
            numPort.Value = Properties.Settings.Default.SelectedPort;
            langNeutralcheckBox.Checked = Properties.Settings.Default.DetectMultipleLanguages;

            initializeToolTips();
            loadVideoDevices();
            loadResolutions();

            // Check latest version
            backgroundWorker1.RunWorkerAsync();
        }

        private void initializeToolTips()
        {
            new ToolTip().SetToolTip(ocrLabel, "Last level information read in.");
            new ToolTip().SetToolTip(deviceLabel, "Available capture devices.");
            new ToolTip().SetToolTip(propertiesButton, "Properties for the selected capture device.");
            new ToolTip().SetToolTip(numPortLabel, "Port to run the Web Server on (used for OBS Browser Source)");
            new ToolTip().SetToolTip(langNeutralcheckBox, "Scan gamefeed for generic button presses in addition to button text.\r\nExperimental feature, may trigger false events.\r\nAlso does not yet support all events.");
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

            resolutionsCombobox.Items.Add(new { Name = "640x480 (low accuracy / low CPU)", Value = new Size(640, 480) });
            resolutionsCombobox.Items.Add(new { Name = "1280x720", Value = new Size(1280, 720) });
            resolutionsCombobox.Items.Add(new { Name = "1920x1080 (high accuracy / high CPU)", Value = new Size(1920, 1080) });

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
                SMMServer.port = decimal.ToUInt16(numPort.Value);
                log.Info(string.Format("Start Web Server on http://localhost:{0}/", SMMServer.port));
                SMMServer.Start();

                if (Properties.Settings.Default.WarpWorldEnabled)
                {
                    WarpWorld = new WarpWorldAPI(Properties.Settings.Default.WarpWorldUsername, Properties.Settings.Default.WarpWorldToken);
                } else
                {
                    WarpWorld = null;
                }
                processor = new VideoProcessor(deviceComboBox.SelectedIndex, SelectedResolution);

                processor.TemplateMatch += broadcastTemplateMatch;

                processor.TemplateMatch += previewMatch;
                processor.NewFrame += previewNewFrame;


                processor.LevelScreen += Processor_LevelScreen;
                processor.ClearScreen += Processor_ClearScreen;

                processor.Exit += clearJsonOnEvent;
                processor.Skip += clearJsonOnEvent;
                processor.GameOver += clearJsonOnEvent;

                processor.ClearScreen += warpWorldCallback;
                processor.Exit += warpWorldCallback;





                processor.Start();
                lockForm();
            }
            catch (Exception ex)
            {
                processException("Error starting video device", ex);
            }
        }

        private void clearJsonOnEvent(object sender, VideoProcessor.TemplateMatchEventArgs e)
        {
            switch (e.template.eventType)
            {
                case "exit":
                    if (JsonSettings.ClearOnExit) clearJsonFile();
                    break;
                case "skip":
                    if (JsonSettings.ClearOnSkip) clearJsonFile();
                    break;
                case "gameover":
                    if (JsonSettings.ClearOnGameover) clearJsonFile();
                    break;
            }
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
            if (JsonSettings.ClearOnStop)
                clearJsonFile();
        }

        private void lockForm()
        {
            langNeutralcheckBox.Enabled = false;
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
            langNeutralcheckBox.Enabled = true;
            processStatusIcon.BackColor = Color.Red;
            deviceComboBox.Enabled = true;
            resolutionsCombobox.Enabled = true;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            processingLabel.Visible = false;
            numPort.Enabled = true;
            webServerAddressStatusLabel.Text = "";
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
            Properties.Settings.Default.DetectMultipleLanguages = langNeutralcheckBox.Checked;
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

        private void langNeutralcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DetectMultipleLanguages = langNeutralcheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            checkLatestVersion();
        }

        /// <summary>
        /// Check via github API if there is a new version available. If so alert the user, if they have not been alerted yet.
        /// </summary>
        private void checkLatestVersion()
        {
            try
            {
                string githubApi = @"https://api.github.com/repos/dram55/MarioMaker2OCR/releases/latest";
                string latestReleaseUrl = @"https://github.com/dram55/MarioMaker2OCR/releases";

                // Github requires User-Agent in header
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(githubApi);
                request.UserAgent = "MarioMaker2OCR";

                System.Net.WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string json = reader.ReadToEnd();
                    dynamic jsonObject = Newtonsoft.Json.Linq.JObject.Parse(json);
                    string latestVersion = jsonObject.name;
                    string lastVersionWarned = Properties.Settings.Default.NewVersionWarning;

                    // Are they on the latest version?
                    if (latestVersion != null && latestVersion != this.CurrentVersion)
                    {
                        // Have they already been warned about this version? Only notify 1x per version
                        if (lastVersionWarned != latestVersion)
                        {
                            Properties.Settings.Default.NewVersionWarning = latestVersion;
                            Properties.Settings.Default.Save();

                            string releaseNotes = jsonObject.body?.ToString().Replace("<br>","\r\n - ");
                            string message = $"New version {latestVersion} is available! Do you want to download now?\r\n\r\n";

                            // Add release notes and remove commitId, if exists
                            if (releaseNotes != null)
                                message += $"Includes Changes:\r\n - {Regex.Replace(releaseNotes, @"\(\w{7}\)", "")}";

                            DialogResult result = MessageBox.Show(message, "New Version Available",MessageBoxButtons.OKCancel,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.OK)
                            {
                                Process.Start(latestReleaseUrl);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout about = new FormAbout(CurrentVersion);
            about.ShowDialog();
		}

        private void WarpWorldSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (warpworldForm.IsDisposed)
            {
                warpworldForm = new FormWarpWorld();
            }
            warpworldForm.Show();
            warpworldForm.BringToFront();
        }

        private void selectOutputFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a location to save the JSON output file.";

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                jsonOutputFolder = dialog.SelectedPath;
            }
        }

        private void clearFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearJsonFile();
        }

        private void clearJsonFile()
        {
            Level emptyLevel = new Level();
            writeLevelToFile(emptyLevel);
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormJsonSettings jsonSettings = new FormJsonSettings();
            jsonSettings.ShowDialog();
            jsonSettings.BringToFront();
        }

        private void broadcastTemplateMatch(object sender, VideoProcessor.TemplateMatchEventArgs e)
        {
            SMMServer.BroadcastEvent(e.template.eventType);
        }

        private void previewMatch(object sender, VideoProcessor.TemplateMatchEventArgs e)
        {
            var boundary = ImageLibrary.ChangeSize(new Rectangle(e.location, e.template.template.Size), processor.TEMPLATE_FRAME_SIZE, processor.frameSize);
            previewer.SetLastMatch(e.frame, new Rectangle[] { boundary });
            
        }
        private void previewNewFrame(object sender, VideoProcessor.NewFrameEventArgs e)
        {
            previewer.SetLiveFrame(e.frame);
        }

        private void Processor_ClearScreen(object sender, VideoProcessor.ClearScreenEventArgs e)
        {
            if (Properties.Settings.Default.WarpWorldEnabled) WarpWorld?.win();
            SMMServer.BroadcastDataEvent("clear", e.clearTime);
            // TODO: Send WR/First Clear here also since we have that in the event
        }

        private void Processor_LevelScreen(object sender, VideoProcessor.LevelScreenEventArgs e)
        {
            previewer.SetLastMatch(e.frame);
            writeLevelToFile(e.levelInfo);
            SMMServer.BroadcastLevel(e.levelInfo);
            BeginInvoke((MethodInvoker)(() => ocrTextBox.Text = e.levelInfo.code + "  |  " + e.levelInfo.author + "  |  " + e.levelInfo.name));
        }

        private void warpWorldCallback(object sender, VideoProcessor.TemplateMatchEventArgs e)
        {
            if (!Properties.Settings.Default.WarpWorldEnabled) return;
            if (e.template.eventType == "exit") WarpWorld?.lose();
        }

        private void warpWorldCallback(object sender, VideoProcessor.ClearScreenEventArgs e)
        {
            if (!Properties.Settings.Default.WarpWorldEnabled) return;
            WarpWorld?.win();
        }
    }
}
