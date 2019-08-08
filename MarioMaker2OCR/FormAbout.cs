using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarioMaker2OCR
{
    public partial class FormAbout : Form
    {
        public FormAbout(string version)
        {
            InitializeComponent();
            versionLabel.Text = version;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/dram55/MarioMaker2OCR");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
