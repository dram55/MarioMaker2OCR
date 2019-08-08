using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2OCR.Objects
{
    public static class JsonSettings
    {
        private static bool? clearOnExit;
        public static bool ClearOnExit
        {
            get
            {
                if (clearOnExit == null)
                {
                    clearOnExit = Properties.Settings.Default.jsonClearOnExit;
                }
                return (bool)clearOnExit;
            }
            set
            {
                clearOnExit = value;
                Properties.Settings.Default.jsonClearOnExit = value;
                Properties.Settings.Default.Save();
            }
        }
        private static bool? clearOnSkip;
        public static bool ClearOnSkip
        {
            get
            {
                if (clearOnSkip == null)
                {
                    clearOnSkip = Properties.Settings.Default.jsonClearOnSkip;
                }
                return (bool)clearOnSkip;
            }
            set
            {
                clearOnSkip = value;
                Properties.Settings.Default.jsonClearOnSkip = value;
                Properties.Settings.Default.Save();
            }
        }
        private static bool? clearOnGameover;
        public static bool ClearOnGameover
        {
            get
            {
                if (clearOnGameover == null)
                {
                    clearOnGameover = Properties.Settings.Default.jsonClearOnGameover;
                }
                return (bool)clearOnGameover;
            }
            set
            {
                clearOnGameover = value;
                Properties.Settings.Default.jsonClearOnGameover = value;
                Properties.Settings.Default.Save();
            }
        }
        private static bool? clearOnStop;
        public static bool ClearOnStop
        {
            get
            {
                if (clearOnStop == null)
                {
                    clearOnStop = Properties.Settings.Default.jsonClearOnStop;
                }
                return (bool)clearOnStop;
            }
            set
            {
                clearOnStop = value;
                Properties.Settings.Default.jsonClearOnStop = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
