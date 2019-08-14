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
    public class EventTemplate : IDisposable
    {
        public Image<Gray, byte> template { get; }
        public double threshold { get; }
        public string eventType { get; }
        public string filename { get; }
        public Rectangle[] regions { get; }
        public double scale { get; }

        bool disposed = false; 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                template.Dispose();
            }
            disposed = true;
        }

        public EventTemplate(string fn, string type, double thresh)
        {

            template = new Image<Gray, byte>(fn);
            threshold = thresh;
            eventType = type;
            filename = fn;
            scale = 1;
        }

        public EventTemplate(string fn, string type, double thresh, Rectangle[] ROIs)
        {
            template = new Image<Gray, byte>(fn);
            threshold = thresh;
            eventType = type;
            filename = fn;
            regions = ROIs;
            scale = 1;
        }

        public EventTemplate(string fn, string type, double thresh, Rectangle[] ROIs, double scale)
        {
            template = new Image<Gray, byte>(fn);
            threshold = thresh;
            eventType = type;
            filename = fn;
            regions = ROIs;
            this.scale = scale;
        }

        public Point getLocation(Image<Gray, byte> frame)
        {
            for (double i = 1; i <= scale; i = i + .05d)
            {
                if (regions == null || regions.Length == 0)
                {
                    Point ret = getLocation(frame, Rectangle.Empty, i);
                    if (!ret.IsEmpty)
                    {
                        return ret;
                    }
                }
                else
                {
                    foreach (Rectangle roi in regions)
                    {
                        Point ret = getLocation(frame, roi, i);
                        if (!ret.IsEmpty)
                        {
                            ret.X += roi.X;
                            ret.Y += roi.Y;
                            return ret;
                        }
                    }
                }
            }

            return Point.Empty;
        }

        public Point getLocation(Image<Gray, byte> frame, Rectangle roi, double scale)
        {
            Image<Gray, byte> templateResized;
            if (scale > 1)
                templateResized = template.Resize(scale, Emgu.CV.CvEnum.Inter.Cubic);
            else
                templateResized = template;
            frame.ROI = roi;
            Image<Gray, float> match = frame.MatchTemplate(templateResized, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            match.MinMax(out _, out double[] max, out _, out Point[] maxLoc);
            if (max[0] < threshold) return Point.Empty;
            return maxLoc[0];
        }
    }
}
