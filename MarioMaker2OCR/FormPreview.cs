using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace MarioMaker2OCR
{
    public partial class FormPreview : Form
    {
        public FormPreview()
        {
            InitializeComponent();
        }

        public void SetLiveFrame(Mat frame)
        {
            if (!this.Disposing && this.Visible)
            {
                try
                {
                    //Live Frames are refs to the Form1 copy, it disposes of them once they leave the FrameBuffer
                    imgLiveFrame.Image = frame;
                }
                catch (ObjectDisposedException)
                {
                    // Happens if the window is closed while the ImageBox is processes the Mat
                }
            }
        }

        public void SetLastMatch(Mat frame, Rectangle[] boundaries)
        {
            if (!this.Disposing && this.Visible)
            {
                MCvScalar borderColor = new Bgr(Color.Red).MCvScalar;
                foreach (Rectangle r in boundaries)
                {
                    Emgu.CV.CvInvoke.Rectangle(frame, r, borderColor, 2);
                }

                //frame.Save("match_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png"); // XXX: Useful for debugging template false-positives.

                try
                {
                    imgLastMatch.Image?.Dispose();
                    imgLastMatch.Image = frame;
                }
                catch (ObjectDisposedException)
                {
                    // Happens if the window is closed while the ImageBox is processes the Mat
                }
            }
        }
    }
}
