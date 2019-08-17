using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace MarioMaker2OCR.Objects
{
    class EventColorTemplate : AbstractTemplate, IDisposable
    {
        public override string eventType { get; }
        private Size _size;
        public override Size size {
            get
            {
                if (_size == null) return Size.Empty;
                else return _size;
            }
        }
        public override double threshold { get; }

        public Rectangle[] regions { get; }
        public int color { get; }

        /// <summary>
        /// Search for a color in a given frame
        /// </summary>
        /// <param name="type">Name of this event</param>
        /// <param name="color">Hue of this template - HSL(A)</param>
        /// <param name="threshold">% of pixels the color must be present in a region (0 - 1)</param>
        /// <param name="regions">Region(s) of the frame to look for the color</param>
        public EventColorTemplate(string type, int color, double threshold, Rectangle[] regions)
        {
            if (regions == null || regions.Length == 0)
                throw new ArgumentException("EventColorTemplate must have at least 1 valid region.");

            this.threshold = threshold;
            this.eventType = type;
            this.color = color;
            this.regions = regions;
        }

        public override Point getLocation(IInputArray frame)
        {
            if (frame is Image<Bgr, byte>)
            {
                Image<Bgr, byte> colorFrame = (Image<Bgr, byte>)frame;

                foreach (Rectangle roi in regions)
                {
                    Point ret = getLocation(colorFrame, roi);
                    if (!ret.IsEmpty)
                    {
                        ret.X += roi.X;
                        ret.Y += roi.Y;
                        return ret;
                    }
                }
            }
            return Point.Empty;
        }

        private Point getLocation(Image<Bgr, byte> frame, Rectangle roi)
        {
            frame.ROI = roi;

            // Get hues of frame
            Dictionary<int, int> hues = VideoProcessor.getHues(frame.Mat.GetRawData(), frame.ROI.Size, 3);
            try
            {
                int colorCoverage;

                // Check for nearby hues from this.color
                for (int hue = this.color + 2; hue > 0 && hue >= this.color - 2; hue--)
                {
                    if (hues.TryGetValue(hue, out colorCoverage))
                    {
                        if (colorCoverage > (threshold * 100))
                        {
                            _size = new Size(roi.Size.Width, roi.Size.Height);
                            return roi.Location;
                        }
                    }
                }

                return Point.Empty;
            }
            finally
            {
                frame.ROI = Rectangle.Empty;
            }
        }

        // Implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {}
    }
}
