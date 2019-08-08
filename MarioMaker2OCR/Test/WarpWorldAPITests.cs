using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarioMaker2OCR;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Runtime.InteropServices;
using System.IO;

namespace MarioMaker2OCR.Test
{
    [TestClass]
    [Ignore("Ignore Warp World tests to avoid hitting the API unnecessarily")]
    public class WarpWorldAPITests
    {
        [TestMethod]
        public void FailsWithBadCreds()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                new WarpWorldAPI("test-username", "bad-token");
            });
        }

        [TestMethod]
        public void CanRetrieveQueueId()
        {
            var ww = new WarpWorldAPI(Properties.Settings.Default.WarpWorldUsername, Properties.Settings.Default.WarpWorldToken);
            var id = ww.getQueueId();
            Assert.IsTrue(id > 0);
        }

    }
}
