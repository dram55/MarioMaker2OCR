using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Runtime.InteropServices;
using System.IO;

namespace MarioMaker2OCR.Test
{
    [TestClass]
    public class ClearScreenTests
    {
        [TestMethod]
        public void ReturnsTrueOnClearFrames()
        {
            string[] filePaths = Directory.GetFiles("./Test/testdata/frames/1080/", "clear_*.png", SearchOption.TopDirectoryOnly);

            byte[] data = new byte[1080 * 1920 * 3];
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

            foreach (string fn in filePaths)
            {
                var testFrame = new Image<Bgr, byte>(fn);

                // Test frame MUST be 1920x1080 otherwise a black frame will be returned with no errors or warnings
                Assert.IsTrue(testFrame.Size.Width == 1920);
                Assert.IsTrue(testFrame.Size.Height == 1080);

                Mat currentFrame = new Mat(testFrame.Size, DepthType.Cv8U, 3, dataHandle.AddrOfPinnedObject(), testFrame.Width * 3);
                testFrame.Mat.CopyTo(currentFrame);
                var hues = VideoProcessor.getHues(data, testFrame.Size);
                Assert.IsTrue(VideoProcessor.isClearFrame(hues));
            }

            dataHandle.Free();
        }

        [TestMethod]
        public void ReturnsFalseOnTemplateFrames()
        {
            // All the of the test frames in ./1080 should be from game stills so never black
            string[] filePaths = Directory.GetFiles("./Test/testdata/frames/1080/", "*.png", SearchOption.TopDirectoryOnly);
            byte[] data = new byte[1080 * 1920 * 3];
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

            foreach (string fn in filePaths)
            {
                if (fn.Contains("clear_")) continue;
                var testFrame = new Image<Bgr, byte>(fn);

                // Test frame MUST be 1920x1080 otherwise a black frame will be returned with no errors or warnings
                Assert.IsTrue(testFrame.Size.Width == 1920);
                Assert.IsTrue(testFrame.Size.Height == 1080);

                Mat currentFrame = new Mat(testFrame.Size, DepthType.Cv8U, 3, dataHandle.AddrOfPinnedObject(), testFrame.Width * 3);
                testFrame.Mat.CopyTo(currentFrame);
                var hues = VideoProcessor.getHues(data, testFrame.Size);
                Assert.IsFalse(VideoProcessor.isClearFrame(hues));
            }
        }

        [TestMethod]
        public void ReturnsFalseOngameplay()
        {
            // All the of the test frames in ./1080 should be from game stills so never black
            string[] filePaths = Directory.GetFiles("./Test/testdata/frames/1080/gameplay", "*.png", SearchOption.TopDirectoryOnly);
            byte[] data = new byte[1080 * 1920 * 3];
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

            foreach (string fn in filePaths)
            {
                if (fn.Contains("clear_")) continue;
                var testFrame = new Image<Bgr, byte>(fn);

                // Test frame MUST be 1920x1080 otherwise a black frame will be returned with no errors or warnings
                Assert.IsTrue(testFrame.Size.Width == 1920);
                Assert.IsTrue(testFrame.Size.Height == 1080);

                Mat currentFrame = new Mat(testFrame.Size, DepthType.Cv8U, 3, dataHandle.AddrOfPinnedObject(), testFrame.Width * 3);
                testFrame.Mat.CopyTo(currentFrame);
                var hues = VideoProcessor.getHues(data, testFrame.Size);
                Assert.IsFalse(VideoProcessor.isClearFrame(hues));
            }
        }

        [TestMethod]
        public void ReadCorrectClearTime()
        {
            string[] filePaths = Directory.GetFiles("./Test/testdata/frames/1080/clears", "*.png", SearchOption.TopDirectoryOnly);
            string ocrErrors = "";

            foreach (var file in filePaths)
            {
                // Grab expected time which should be in filename (ex: clear_time_usa_0109645.png)
                int timeIdx = file.LastIndexOf("_") + 1;
                string expectedTime = file.Substring(timeIdx, 7);
                expectedTime = $"{expectedTime.Substring(0, 2)}'{expectedTime.Substring(2, 2)}\"{expectedTime.Substring(4, 3)}";

                using (var testFrame = new Image<Bgr, byte>(file))
                {
                    string ocrClearTime = OCRLibrary.GetClearTimeFromFrame(testFrame, false);
                    if (expectedTime != ocrClearTime)
                    {
                        ocrErrors += $"\r\nClear time OCR: ({ocrClearTime}) does not match expected: ({expectedTime})";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(ocrErrors))
                Assert.Fail(ocrErrors);
        }
    }
}
