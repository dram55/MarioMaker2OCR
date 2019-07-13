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

        public Size SelectedResolution => (resolutionsCombobox.SelectedItem as dynamic)?.Value;
        public DsDevice SelectedDevice => (deviceComboBox.SelectedItem as dynamic)?.Value;

        public Form1()
        {
            InitializeComponent();

            outputFolderTextbox.Text = Properties.Settings.Default.OutputFolder;

            processVideoFrameTimer = new System.Timers.Timer(800);
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

        private void processVideoFrame()
        {
            Mat currentFrame = new Mat();
            try
            {
                if (!videoDevice.IsOpened) return;
                videoDevice.Retrieve(currentFrame);
                if (currentFrame.Bitmap == null)
                {
                    throw new Exception("Unable to retrieve the current video frame. Device could be in use by another program.");
                }
                double imageMatchPercent = ImageLibrary.CompareImages(currentFrame, levelSelectScreen);
                BeginInvoke((MethodInvoker)(() => percentMatchLabel.Text = String.Format("{0:P2}", imageMatchPercent)));

                if (imageMatchPercent > .94)
                {
                    BeginInvoke((MethodInvoker)(() => processingLevelLabel.Visible = true));

                    Level level = getLevelFromCurrentFrame(currentFrame.ToImage<Bgr, byte>());
                    writeLevelToFile(level);
                    BeginInvoke((MethodInvoker)(() => ocrTextBox.Text = level.code + "  |  " + level.author + "  |  " + level.name));

                    // Sleep 4 seconds to prevent processing same frame twice
                    Thread.Sleep(4000);

                    // Read to clear buffer & return to most recent frame
                    videoDevice.Retrieve(currentFrame);
                    BeginInvoke((MethodInvoker)(() => processingLevelLabel.Visible = false));
                }
            }
            catch (Exception ex)
            {
                processException("Error Processing Video Frame", ex);
            }
            finally
            {
                currentFrame.Dispose();
            }
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

                log.Info($"Connecting to {deviceComboBox.SelectedIndex} - {deviceComboBox.SelectedItem}");
                log.Info($"Using Resolution: {videoDevice.Width}x{videoDevice.Height}");

                lockForm();
            }
            catch (Exception ex)
            {
                processException("Error starting video device", ex);
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
            percentMatchLabel.Text = "";
            processStatusIcon.BackColor = Color.Green;
        }

        private void unlockForm()
        {
            processVideoFrameTimer.Stop();
            processStatusIcon.BackColor = Color.Red;
            percentMatchLabel.Text = "";
            deviceComboBox.Enabled = true;
            resolutionsCombobox.Enabled = true;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            processingLevelLabel.Visible = false;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            videoDevice.Stop();
            videoDevice.Dispose();
            BeginInvoke(new MethodInvoker(() => unlockForm()));
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
            Properties.Settings.Default.Save();
        }
    }
}
