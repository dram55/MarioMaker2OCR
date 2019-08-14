using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2OCR.Test
{
    [TestClass]
    public class StatsTrackerTests
    {
        [TestMethod]
        public void CanAddEvents()
        {
            var tracker = new StatsTracker();

            Assert.AreEqual(tracker.count(), 0);

            tracker.addEvent("testEvent");
            Assert.AreEqual(tracker.count(), 1);

            tracker.addEvent("testEventWithData", "[test data]");
            Assert.AreEqual(tracker.count(), 2);

            for(int i=0;i<20;i++)
            {
                if(i%2==0)
                {
                    tracker.addEvent("testEventWithData", String.Format("test{0}", i));
                } else
                {
                    tracker.addEvent("testEvent");
                }
            }

            Assert.AreEqual(tracker.count(), 22);
            Assert.AreEqual(tracker.count("testEvent"), 11);
            Assert.AreEqual(tracker.count("testEventWithData"), 11);

            var result = tracker.latest("testEventWithData");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.data, "test18");
        }
    }
}
