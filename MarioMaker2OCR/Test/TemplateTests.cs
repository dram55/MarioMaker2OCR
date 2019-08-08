using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emgu.CV;
using Emgu.CV.Structure;
using MarioMaker2OCR.Objects;
using System.IO;
using System.Drawing;

namespace MarioMaker2OCR.Test
{
    [TestClass]
    public class TemplateTests
    {
        public static readonly string frameDir = "./Test/testdata/frames";
        private readonly EventTemplate[] templates = new EventTemplate[] {
            new EventTemplate("./templates/480/exit.png", "exit", 0.8, new Rectangle[] {
                new Rectangle(new Point(400,330), new Size(230, 65)), //Pause Menu
                new Rectangle(new Point(410,225), new Size(215, 160)), //Clear Screen
            }),
            new EventTemplate("./templates/480/quit.png", "exit", 0.9, new Rectangle[] {
                new Rectangle(new Point(408,338), new Size(224, 70)) //Pause Menu
            }),
            new EventTemplate("./templates/480/quit_full.png", "exit", 0.8, new Rectangle[] {
                new Rectangle(new Point(195,225), new Size(215, 160)), //Clear Screen
            }),
            new EventTemplate("./templates/480/startover.png", "restart", 0.8, new Rectangle[] {
                new Rectangle(new Point(400,275), new Size(230, 65)), //Pause Menu
                new Rectangle(new Point(195,225), new Size(215, 160)), //Clear Screen
            }),
            new EventTemplate("./templates/480/death_big.png", "death", 0.8),
            new EventTemplate("./templates/480/death_small.png", "death", 0.8),
            new EventTemplate("./templates/480/death_partial.png", "death", 0.9),
            new EventTemplate("./templates/480/gameover.png", "gameover", 0.8, new Rectangle[] {
                new Rectangle(new Point(187,195), new Size(270, 100))
            }),
            new EventTemplate("./templates/480/skip.png", "skip", 0.85, new Rectangle[] {
                new Rectangle(new Point(308,200), new Size(25, 37))
            })
        };

        private readonly EventTemplate[] clearTemplates = new EventTemplate[]
        {
            new EventTemplate("./templates/480/worldrecord.png", "worldrecord", 0.8, new Rectangle[] {
                new Rectangle(new Point(445,85), new Size(115, 130)),
            }),
            new EventTemplate("./templates/480/firstclear.png", "firstclear", 0.8, new Rectangle[] {
                new Rectangle(new Point(445,85), new Size(115, 130)),
            })
        };

        [TestMethod]
        public void ReturnsEmptyAgainstStatic()
        {
            string fn = frameDir + "/480/static.png";
            var testFrame = new Image<Gray, byte>(fn);
            foreach (EventTemplate currentTemplate in templates)
            {
                var result = currentTemplate.getLocation(testFrame);
                Assert.IsTrue(result.IsEmpty, String.Format("Template {0} matched on {1} at ({2}, {3})", currentTemplate.filename, fn, result.X, result.Y));
            }
        }

        [TestMethod]
        public void ReturnsEmptyAgainstBlackScreen()
        {
            string fn = frameDir + "/480/black.png";
            var testFrame = new Image<Gray, byte>(fn);
            foreach (EventTemplate currentTemplate in templates)
            {
                var result = currentTemplate.getLocation(testFrame);
                Assert.IsTrue(result.IsEmpty, String.Format("Template {0} matched on {1} at ({2}, {3})", currentTemplate.filename, fn, result.X, result.Y));
            }
        }

        [TestMethod]
        public void ReturnsEmptyAgainstWhiteScreen()
        {
            string fn = frameDir + "/480/white.png";
            var testFrame = new Image<Gray, byte>(fn);
            foreach (EventTemplate currentTemplate in templates)
            {
                var result = currentTemplate.getLocation(testFrame);
                Assert.IsTrue(result.IsEmpty, String.Format("Template {0} matched on {1} at ({2}, {3})", currentTemplate.filename, fn, result.X, result.Y));
            }
        }

        //The Detection tests find the template in the array rather than declaring it locally so that only one location needs be updated

        [TestMethod]
        public void DetectsDeathBig()
        {
            string[] files = new string[]
            {
                "/1080/death0.png",
                "/1080/death1.png",
                "/1080/death_upsidedown.png",
            };
            foreach (var t in templates)
            {
                if (!t.filename.EndsWith("death_big.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectsDeathSmall()
        {
            //Finding the template in the array rather than declaring it so if threshold changes or something its only one change
            string[] files = new string[]
            {
                "/1080/death0.png",
                "/1080/death1.png",
                "/1080/death_upsidedown.png",
            };
            foreach (var t in templates)
            {
                if (!t.filename.EndsWith("death_small.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectsDeathPartial()
        {
            //Finding the template in the array rather than declaring it so if threshold changes or something its only one change
            string[] files = new string[]
            {
                "/1080/death0.png",
                "/1080/death1.png",
                //"/1080/death_upsidedown.png", //I'm not sure partials even happen on upside down levels, it doens't match the upside down death regardless.
            };
            foreach (var t in templates)
            {
                if (!t.filename.EndsWith("death_partial.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectsStartOver()
        {
            string[] files = new string[]
            {
                "/1080/startover_clearscreen.png",
                "/1080/startover_pause.png"
            };
            foreach (var t in templates)
            {
                if (!t.filename.EndsWith("startover.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectsExit()
        {
            string[] files = new string[]
            {
                "/1080/exit_clearscreen.png",
                "/1080/exit_pause.png"
            };
            foreach (var t in templates)
            {
                if (!t.filename.EndsWith("exit.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    Assert.IsFalse(t.getLocation(frame).IsEmpty);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectQuit()
        {
            string[] files = new string[]
            {
                "/480/quit.png"
            };
            foreach (var t in templates)
            {
                if (!t.filename.EndsWith("quit.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn);
                    Assert.IsFalse(t.getLocation(frame).IsEmpty);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectWorldRecord()
        {
            string[] files = new string[]
            {
                "/1080/worldrecord.png",
            };
            foreach (var t in clearTemplates)
            {
                if (!t.filename.EndsWith("worldrecord.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    Assert.IsFalse(t.getLocation(frame).IsEmpty);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectGameOver()
        {
            string[] files = new string[]
            {
                "/480/gameover.png",
            };
            foreach (var t in clearTemplates)
            {
                if (!t.filename.EndsWith("gameover.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn);
                    Assert.IsFalse(t.getLocation(frame).IsEmpty);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void DetectFirstClear()
        {
            string[] files = new string[]
            {
                "/1080/firstclear.png",
            };
            foreach (var t in clearTemplates)
            {
                if (!t.filename.EndsWith("firstclear.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    Assert.IsFalse(t.getLocation(frame).IsEmpty);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void WorldRecordReturnEmptyOnFirstClear()
        {
            string[] files = new string[]
            {
                "/1080/firstclear.png",
            };
            foreach (var t in clearTemplates)
            {
                if (t.filename.EndsWith("firstclear.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    Assert.IsTrue(t.getLocation(frame).IsEmpty, "World record template matched on the first clear screenshot. Not expected.");
                }
            }
        }

        [TestMethod]
        public void FirstClearReturnEmptyOnWorldRecord()
        {
            string[] files = new string[]
            {
                "/1080/worldrecord.png",
            };
            foreach (var t in clearTemplates)
            {
                if (t.filename.EndsWith("worldrecord.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    Assert.IsTrue(t.getLocation(frame).IsEmpty, "First clear template matched on the World Record screenshot.Not expected.");
                }
            }
        }

        [TestMethod]
        public void DetectSkip()
        {
            string[] files = new string[]
            {
                "/1080/skip.png",
            };
            foreach (var t in clearTemplates)
            {
                if (!t.filename.EndsWith("skip.png")) continue;
                foreach (var fn in files)
                {
                    var frame = new Image<Gray, byte>(frameDir + fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                    Assert.IsFalse(t.getLocation(frame).IsEmpty);
                    var result = t.getLocation(frame);
                    Assert.IsFalse(result.IsEmpty, String.Format("Template {0} did not match {1}", t.filename, fn));
                }
            }
        }

        [TestMethod]
        public void ReturnsEmptyOnGameplay()
        {
            string[] filePaths = Directory.GetFiles(frameDir + "/1080/gameplay", "*.png", SearchOption.TopDirectoryOnly);
            foreach(var fn in filePaths)
            {
                var frame = new Image<Gray, byte>(fn).Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);
                foreach(var t in templates)
                {
                    var result = t.getLocation(frame);
                    Assert.IsTrue(result.IsEmpty, String.Format("Template {0} matched on {1} at ({2}, {3})", t.filename, fn, result.X, result.Y));
                }
            }
        }
    }
}
