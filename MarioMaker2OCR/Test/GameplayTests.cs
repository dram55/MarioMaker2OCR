using Emgu.CV;
using Emgu.CV.Structure;
using MarioMaker2OCR.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2OCR.Test
{
    [TestClass]
    public class GameplayTests
    {
        [TestMethod]
        public void TenMinuteGameplayTest()
        {
            // Commented out for now since this is above githubs 100MB filesize limit - even when compressed. 
            // Will make a smaller test in the near future.
            //
            // Here is the video: https://drive.google.com/file/d/1NMpA4z2AvedB4bbhtWvXrIlV4QuBxRTC/view?usp=sharing
            //
            //VideoCapture deathVideo = new VideoCapture(@"Test\testdata\video\720\Dram_Gameplay_Test_1.mp4");
            VideoCapture deathVideo = new VideoCapture(@"");
            return;


            // Read first frame, for some reason the Bitmap is null - so we don't want this going to the VideoProcessor
            deathVideo.Read(new Mat());

            // Create and start the video processor object
            int deathCount = 0;
            int restartCount = 0;
            int wrCount = 0;
            int firstClearCount = 0;
            int exitCount = 0;
            int clearCount = 0;

            int unknownTemplateCount = 0;

            List<Level> levels = new List<Level>();

            List<string> expectedClearTimes = new List<string> { "00'09\"588", "00'42\"156", "00'21\"532", "00'38\"019" };
            List<string> actualClearTimes = new List<string>();

            HashSet<string> expectedlevelCodes = new HashSet<string> { "HXL-XFC-5NF", "P7N-9G1-KGF",  "F0V-CCC-QGG", "NC4-N2G-3RF" , "GMB-VS4-PFG", "KWC-KCH-HSG", "SL5-CY5-94G", "C38-TH8-CTG" };
            HashSet<string> actualLevelCodes = new HashSet<string>();

            VideoProcessor mockedVideoProcessor = new VideoProcessor(deathVideo);
            mockedVideoProcessor.ClearScreen += (d, g) =>
            {
                clearCount++;
                actualClearTimes.Add(g.clearTime);
            };

            mockedVideoProcessor.LevelScreen += (d, g) =>
            {
                actualLevelCodes.Add(g.levelInfo.code);
            };

            mockedVideoProcessor.TemplateMatch += (d, g) =>
            {
                switch (g.template.eventType)
                {
                    case "death": deathCount++; break;
                    case "restart": restartCount++; break;
                    case "worldrecord": wrCount++; break;
                    case "firstclear": firstClearCount++; break;
                    case "exit": exitCount++; break;

                    default: unknownTemplateCount++; break;
                }
            };


            mockedVideoProcessor.Start(true);

            Assert.AreEqual(4, deathCount, "Death Check");
            Assert.AreEqual(6, restartCount, "Restart Check");
            Assert.AreEqual(1, wrCount, "World Record Check");
            Assert.AreEqual(1, firstClearCount, "First Clear Check");
            Assert.AreEqual(5, exitCount, "Exit/Quit Check");
            Assert.AreEqual(4, clearCount, "Clear Check");

            for (int i = 0; i < clearCount; i++)
            {
                Assert.AreEqual(expectedClearTimes[i], actualClearTimes[i], "Clear Time Check");
            }

            Assert.AreEqual(expectedlevelCodes.Count, actualLevelCodes.Count, "Level Screen Count Check");
            for (int i = 0; i < expectedlevelCodes.Count; i++)
            {
                Assert.AreEqual(expectedlevelCodes.ToList()[i], actualLevelCodes.ToList()[i], "Level Code Check");
            }

            Console.WriteLine("Unknown Template Count: " + unknownTemplateCount);
            
        }
    }
}
