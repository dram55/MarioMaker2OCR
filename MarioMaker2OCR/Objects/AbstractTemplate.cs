using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2OCR.Objects
{
    public abstract class AbstractTemplate
    {
        public abstract string eventType { get; }
        public abstract double threshold { get; }
        public abstract Size size { get; }
        public abstract Point getLocation(IInputArray frame);
    }
}
