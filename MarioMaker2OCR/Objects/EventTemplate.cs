using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace MarioMaker2OCR.Objects
{
    class EventTemplate
    {
        public Image<Gray, byte> template { get; }
        public double threshold { get; }
        public string eventType { get; }

        public EventTemplate(string filename, string type, double thresh)
        {
            template = new Image<Gray, byte>(filename);
            threshold = thresh;
            eventType = type;
        }

        public Point getLocation(Image<Gray, byte> frame)
        {
            Image<Gray, float> match = frame.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            match.MinMax(out _, out double[] max, out _, out Point[] maxLoc);
            if (max[0] < threshold) return Point.Empty;
            return maxLoc[0];
        }
    }
}
