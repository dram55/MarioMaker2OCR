using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2OCR
{
    public static class OCRLibrary
    {
        private const string tesserectLibrary = "tessdata";

        /// <summary>
        /// Get string from image using Tesserect Library for OCR
        /// Only use eng+jpn language
        /// </summary>
        public static string GetStringFromImage(Image<Gray, byte> image)
        {
            using (Tesseract r = new Tesseract(tesserectLibrary, "eng+jpn", OcrEngineMode.TesseractOnly))
            {
                r.SetImage(image);
                r.Recognize();
                return r.GetUTF8Text().Trim();
            }
        }

        /// <summary>
        /// Get string from image using Tesserect Library for OCR
        /// Only use eng language and a subset of characters representing MM2 Level Codes
        /// </summary>
        public static string GetStringFromLevelCodeImage(Image<Gray, byte> image)
        {
            using (Tesseract r = new Tesseract(tesserectLibrary, "eng", OcrEngineMode.TesseractOnly))
            {
                r.SetVariable("tessedit_char_whitelist", "-0123456789ABCDEFGHJKLMNPQRSTUVWXY");
                r.SetImage(image);
                r.Recognize();
                return r.GetUTF8Text().Trim().Replace(" ", "");
            }
        }
    }
}
