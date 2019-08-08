using System;
using System.Windows.Forms;
using MarioMaker2OCR.Objects;

namespace MarioMaker2OCR
{
    public partial class FormJsonSettings : Form
    {
        public FormJsonSettings()
        {
            InitializeComponent();
            loadCheckboxStateFromSettings();
        }

        private void loadCheckboxStateFromSettings()
        {
            exitCheckbox.Checked = JsonSettings.ClearOnExit;
            gameoverCheckbox.Checked = JsonSettings.ClearOnGameover;
            skipCheckbox.Checked = JsonSettings.ClearOnSkip;
            stopCaptureCheckbox.Checked = JsonSettings.ClearOnStop;
        }

        // Save
        private void button1_Click(object sender, EventArgs e)
        {
            JsonSettings.ClearOnExit = exitCheckbox.Checked;
            JsonSettings.ClearOnGameover = gameoverCheckbox.Checked;
            JsonSettings.ClearOnSkip = skipCheckbox.Checked;
            JsonSettings.ClearOnStop = stopCaptureCheckbox.Checked;
            this.Close();
        }

        // Cancel
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
