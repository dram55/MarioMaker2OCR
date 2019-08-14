using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarioMaker2OCR.Objects;
using Newtonsoft.Json;

namespace MarioMaker2OCR
{
    class StatsTracker
    {
        private List<StatsEvent> history = new List<StatsEvent>();

        public void addEvent(string eventType)
        {
            addEvent(eventType, "");
        }

        public void addEvent(string eventType, string data)
        {
            var e = new StatsEvent
            {
                type = eventType,
                time = DateTime.Now,
                data = data,
            };
            history.Add(e);
        }

        public void clearHistory()
        {
            history.Clear();
        }

        public StatsEvent latest(string eventType)
        {
            return history.FindLast((StatsEvent e) =>
            {
                return e.type == eventType;
            });
        }

        public int count(string eventType)
        {
            var results = history.FindAll((StatsEvent e) =>
            {
                return e.type == eventType;
            });
            return results.Count;
        }
        public int count()
        {
            return history.Count;
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(history);
        }
    }

    class StatsEvent : DataEventWrapper
    {
        public DateTime time;

    }
}
