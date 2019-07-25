using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using MarioMaker2OCR.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarioMaker2OCR.Test
{
    [TestClass]
    public class LevelScreenTests
    {
        [TestMethod]
        public void ReadCorrectLevelCode()
        {
            string[] filePaths = Directory.GetFiles("./Test/testdata/frames/720/levels", "*.png", SearchOption.TopDirectoryOnly);

            foreach (var file in filePaths)
            {
                int levelIdx = file.LastIndexOf("level.") + 6;
                string actualLevelCode = file.Substring(levelIdx, 11);

                using (var testFrame = new Image<Bgr, byte>(file))
                {
                    Level level = OCRLibrary.GetLevelFromFrame(testFrame);
                    Assert.IsTrue(level.code == actualLevelCode, $"Level code OCR ({level.code}) does not match actual code ({actualLevelCode})");
                }
            }
        }

        [TestMethod]
        public void DetectLevelScreen()
        {
            Assert.Inconclusive("Test Not Implemented.");
        }
    }
}
