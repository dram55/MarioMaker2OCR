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
    public partial class FormWarpWorld : Form
    {
        public FormWarpWorld()
        {
            InitializeComponent();
            txtUsername.Text = Properties.Settings.Default.WarpWorldUsername;
            txtToken.Text = Properties.Settings.Default.WarpWorldToken;
            txtWarpBarURL.Text = String.Format("https://warp.world/warpbar-queue?streamer={0}&key={1}", txtUsername.Text, txtToken.Text);
            chkWarpWorld.Checked = Properties.Settings.Default.WarpWorldEnabled;

            if(!chkWarpWorld.Checked)
            {
                txtWarpBarURL.Enabled = false;
            }
        }

        private void TxtWarpBarURL_TextChanged(object sender, EventArgs e)
        {
            if (txtWarpBarURL.Text.Contains("streamer="))
            {
                txtUsername.Text = txtWarpBarURL.Text.Split(new String[] { "streamer=" }, StringSplitOptions.None)[1].Split('&')[0];
            }

            if (txtWarpBarURL.Text.Contains("key="))
            {
                txtToken.Text = txtWarpBarURL.Text.Split(new String[] { "key=" }, StringSplitOptions.None)[1].Split('&')[0];
            }

            // Possible the token size will change, but its a good validation for now
            if(txtUsername.TextLength > 2 && txtToken.TextLength == 16)
            {
                Properties.Settings.Default.WarpWorldUsername = txtUsername.Text;
                Properties.Settings.Default.WarpWorldToken = txtToken.Text;
                Properties.Settings.Default.Save();

            }
        }

        private void ChkWarpWorld_CheckedChanged(object sender, EventArgs e)
        {
            txtWarpBarURL.Enabled = chkWarpWorld.Checked;
            Properties.Settings.Default.WarpWorldEnabled = chkWarpWorld.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
