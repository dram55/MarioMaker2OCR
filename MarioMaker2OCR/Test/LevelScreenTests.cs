using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using MarioMaker2OCR.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MarioMaker2OCR.Test
{
    [TestClass]
    public class LevelScreenTests
    {
        [TestMethod]
        public void ReadCorrectLevelCode480()
        {
            string result = runOCRTestsForResolution(new Size(640, 480));

            if (!string.IsNullOrWhiteSpace(result))
                Assert.Fail(result);
        }

        [TestMethod]
        public void ReadCorrectLevelCode720()
        {
            string result = runOCRTestsForResolution(new Size(1280, 720));

            if (!string.IsNullOrWhiteSpace(result))
                Assert.Fail(result);
        }

        [TestMethod]
        public void ReadCorrectLevelCode1080()
        {
            string result = runOCRTestsForResolution(new Size(1920, 1080));

            if (!string.IsNullOrWhiteSpace(result))
                Assert.Fail(result);
        }

        private string runOCRTestsForResolution(Size resolution)
        {
            string[] filePaths = Directory.GetFiles("./Test/testdata/frames/720/levels", "*.png", SearchOption.TopDirectoryOnly);
            string ocrErrors = "";

            foreach (var file in filePaths)
            {
                int levelIdx = file.LastIndexOf("level.") + 6;
                string actualLevelCode = file.Substring(levelIdx, 11);

                using (var testFrame = new Image<Bgr, byte>(file))
                {
                    Level level = OCRLibrary.GetLevelFromFrame(testFrame.Resize(resolution.Width, resolution.Height, Inter.Cubic));
                    if (level.code != actualLevelCode)
                    {
                        ocrErrors += $"\r\nLevel code OCR ({level.code}) does not match actual code ({actualLevelCode})";
                    }
                }
            }

            return ocrErrors;
        }

        [TestMethod]
        [Ignore("Test Not Implemented.")]
        public void DetectLevelScreen()
        {
            Assert.Inconclusive("Test Not Implemented.");
        }
    }
}
