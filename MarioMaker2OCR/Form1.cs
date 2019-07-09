using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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

        string LEVEL_JSON_FILE = "ocrLevel.json";

        private static bool ERROR_THROWN = false;

        VideoCapture captureCard;
        Size resolution720 = new Size(1280, 720);
        MCvScalar redRectangle = new MCvScalar(0, 0, 255);
        System.Timers.Timer processVideoFramesTimer;
        private Rectangle levelCodeArea;
        private Rectangle creatorNameArea;
        private Rectangle levelTitleArea;
        Rectangle levelCodeArea720p = new Rectangle(81, 178, 190, 25); // based on 1280x720
        Rectangle creatorNameArea720 = new Rectangle(641, 173, 422, 39); // based on 1280x720
        Rectangle levelTitleArea720 = new Rectangle(100, 92, 1080, 43); // based on 1280x720

        Mat levelSelectScreen;
        Mat levelSelectScreen720 = new Image<Bgr, byte>("referenceImage.jpg").Mat; // based on 1280x720

        public Size SelectedResolution
        {
            get
            {
                return (resolutionsCombobox.SelectedItem as dynamic).Value;
            }
        }

        public DsDevice SelectedCamera
        {
            get
            {
                return (deviceComboBox.SelectedItem as dynamic).Value;
            }
        }

        public Form1()
        {
            InitializeComponent();

            processVideoFramesTimer = new System.Timers.Timer();
            processVideoFramesTimer.Interval = 1500;
            processVideoFramesTimer.Elapsed += new System.Timers.ElapsedEventHandler(readScreenTimer_Tick);

            processStatusIcon.BackColor = Color.Red;
            outputFolderTextbox.Text = Properties.Settings.Default.OutputFolder;

            LoadVideoDevices();
            LoadResolutions();
        }

        private void LoadResolutions()
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

        private void LoadVideoDevices()
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

        private void ProcessVideoFrame()
        {
            Mat currentFrame = new Mat();
            try
            {
                captureCard.Retrieve(currentFrame);
                if (currentFrame.Bitmap == null)
                {
                    throw new Exception("Unable to retrieve the current video frame. Device could be in use by another program.");
                }
                double imageMatchPercent = ImageLibrary.CompareImages(currentFrame, levelSelectScreen);
                 BeginInvoke((MethodInvoker)delegate () { percentMatchLabel.Text = String.Format("{0:P2}", imageMatchPercent); });
                
                if (imageMatchPercent > .94)
                {
                    Level level = GetLevelFromCurrentFrame(currentFrame.ToImage<Bgr, byte>());
                    WriteLevelToFile(level);
                    BeginInvoke((MethodInvoker)delegate () { ocrTextBox.Text = level.code + "  |  " + level.author + "  |  " + level.name; });
                }
            }
            catch (Exception ex)
            {
                ProcessException("Error Processing Video Frame", ex);
            }
            finally
            {
                currentFrame.Dispose();
            }
        }

        private Level GetLevelFromCurrentFrame(Image<Bgr, byte> currentFrame)
        {
            try
            {
                Level ocrLevel = new Level();

                // Level Code
                currentFrame.ROI = levelCodeArea;
                ocrLevel.code = GetStringFromLevelCodeImage(currentFrame);

                // Level Title
                currentFrame.ROI = levelTitleArea;
                ocrLevel.name = GetStringFromImage(currentFrame);

                // Creator Name
                currentFrame.ROI = creatorNameArea;
                ocrLevel.author = GetStringFromImage(currentFrame);

                return ocrLevel;
            }
            catch(Exception ex)
            {
                ProcessException("Error Performing OCR", ex);
            }

            return null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string GetStringFromImage(Image<Bgr, byte> image)
        {
            Image<Gray, byte> ocrReadyImage = ImageLibrary.PrepareImageForOCR(image);
            return OCRLibrary.GetStringFromImage(ocrReadyImage);
        }

        private string GetStringFromLevelCodeImage(Image<Bgr, byte> image)
        {
            Image<Gray, byte> ocrReadyImage = ImageLibrary.PrepareImageForOCR(image);
            return OCRLibrary.GetStringFromLevelCodeImage(ocrReadyImage);
        }

        private void readScreenTimer_Tick(object sender, EventArgs e)
        {
            processVideoFramesTimer.Stop();
            ProcessVideoFrame();
            if (!ERROR_THROWN)
                processVideoFramesTimer.Start();
        }

        private void clearLevelFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Level emptyLevel = new Level();
            LevelWrapper jsonCurrentLevel = new LevelWrapper() { level=emptyLevel };
            WriteLevelToFile(emptyLevel);
        }

        private void WriteLevelToFile(Level level)
        {
            LevelWrapper wrappedLevel = new LevelWrapper() { level = level };
            string json = JsonConvert.SerializeObject(wrappedLevel);
            File.WriteAllText(Path.Combine(outputFolderTextbox.Text, LEVEL_JSON_FILE), json);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (deviceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a video source first.");
                return;
            }
            try
            {
                LockForm();

                // resize reference image based on current resolution
                levelSelectScreen = ImageLibrary.ChangeSize(levelSelectScreen720, resolution720, SelectedResolution);

                // resize rectangles based on current resolution
                levelCodeArea = ImageLibrary.ChangeSize(levelCodeArea720p, resolution720, SelectedResolution);
                creatorNameArea = ImageLibrary.ChangeSize(creatorNameArea720, resolution720, SelectedResolution);
                levelTitleArea = ImageLibrary.ChangeSize(levelTitleArea720, resolution720, SelectedResolution);

                // Set Capture Card Resolution
                InitializeAndStartVideoDevice();

                log.Info($"Connecting to {deviceComboBox.SelectedIndex} - {deviceComboBox.SelectedItem.ToString()}");
                log.Info($"Using Resolution: {captureCard.Width}x{captureCard.Height}");
            }
            catch (Exception ex)
            {
                ProcessException("Error starting camera", ex);
            }
        }

        private void InitializeAndStartVideoDevice()
        {
            captureCard = new VideoCapture(deviceComboBox.SelectedIndex);
            captureCard.SetCaptureProperty(CapProp.FrameHeight, SelectedResolution.Height);
            captureCard.SetCaptureProperty(CapProp.FrameWidth, SelectedResolution.Width);
            captureCard.Start();
        }

        private void LockForm()
        {
            processVideoFramesTimer.Start();
            deviceComboBox.Enabled = false;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            resolutionsCombobox.Enabled = false;
            propertiesButton.Enabled = false;
            ocrTextBox.Text = "";
            percentMatchLabel.Text = "";
            processStatusIcon.BackColor = Color.Green;
        }

        private void UnlockForm()
        {
            processVideoFramesTimer.Stop();
            processStatusIcon.BackColor = Color.Red;
            percentMatchLabel.Text = "";
            deviceComboBox.Enabled = true;
            resolutionsCombobox.Enabled = true;
            startButton.Enabled = true;
            stopButton.Enabled = false;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            captureCard.Stop();
            captureCard.Dispose();
            BeginInvoke(new MethodInvoker(delegate (){ UnlockForm();}));
        }

        private void propertiesButton_Click(object sender, EventArgs e)
        {
            if (deviceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a video source first.");
                return;
            }
            try
            {
                DirectShowLibrary.DisplayPropertyPage(SelectedCamera.Mon, this.Handle);
            }
            catch (Exception ex)
            {
                ProcessException("Error displaying camera properties", ex);
            }
        }

        private void ProcessException(string caption, Exception ex)
        {
            log.Error($"{caption}: {ex.Message}");
            log.Debug(ex.StackTrace);

            ERROR_THROWN = true;
            stopButton_Click(null, null);
            BeginInvoke((MethodInvoker)delegate () { MessageBox.Show(ex.Message, "Error Processing Video", MessageBoxButtons.OK, MessageBoxIcon.Error); });
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.OutputFolder = outputFolderTextbox.Text;
            Properties.Settings.Default.SelectedDevice = (deviceComboBox.SelectedItem as dynamic).Name;
            Properties.Settings.Default.SelectedResolutionIndex = resolutionsCombobox.SelectedIndex;
            Properties.Settings.Default.Save();
        }
    }
}
