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
                imgLiveFrame.Image?.Dispose();
                imgLiveFrame.Image = frame;
            }
        }

        public void SetLastMatch(Mat frame, Rectangle[] boundaries)
        {
            if (!this.Disposing && this.Visible)
            {
                MCvScalar borderColor = new Bgr(Color.Red).MCvScalar;
                foreach(Rectangle r in boundaries)
                {
                    Emgu.CV.CvInvoke.Rectangle(frame, r, borderColor, 2);
                }
                imgLastMatch.Image?.Dispose();
                imgLastMatch.Image = frame;
            }
        }

    }
}
