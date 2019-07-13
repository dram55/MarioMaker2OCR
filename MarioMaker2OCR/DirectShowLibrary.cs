using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;

namespace MarioMaker2OCR
{
    public static class DirectShowLibrary
    {
        public static List<DsDevice> GetCaptureDevices()
        {
            return new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
        }

        #region Display Camera Properties

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)]
                    ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved);

        private static void _DisplayPropertyPage(object filter_or_pin, IntPtr hwndOwner)
        {
            if (filter_or_pin == null)
                return;

            //Get the ISpecifyPropertyPages for the filter
            int hr = 0;

            if (!(filter_or_pin is ISpecifyPropertyPages pProp))
            {
                //If the filter doesn't implement ISpecifyPropertyPages, try displaying IAMVfwCompressDialogs instead!
                if (filter_or_pin is IAMVfwCompressDialogs compressDialog)
                {
                    hr = compressDialog.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                }
                return;
            }

            string caption = string.Empty;

            if (filter_or_pin is IBaseFilter)
            {
                //Get the name of the filter from the FilterInfo struct
                IBaseFilter as_filter = filter_or_pin as IBaseFilter;
                FilterInfo filterInfo;
                hr = as_filter.QueryFilterInfo(out filterInfo);
                DsError.ThrowExceptionForHR(hr);

                caption = filterInfo.achName;

                if (filterInfo.pGraph != null)
                {
                    Marshal.ReleaseComObject(filterInfo.pGraph);
                }
            }
            else
            if (filter_or_pin is IPin)
            {
                //Get the name of the filter from the FilterInfo struct
                IPin as_pin = filter_or_pin as IPin;
                PinInfo pinInfo;
                hr = as_pin.QueryPinInfo(out pinInfo);
                DsError.ThrowExceptionForHR(hr);

                caption = pinInfo.name;
            }

            // Get the propertypages from the property bag
            DsCAUUID caGUID;
            hr = pProp.GetPages(out caGUID);
            DsError.ThrowExceptionForHR(hr);

            // Create and display the OlePropertyFrame
            object oDevice = (object)filter_or_pin;
            hr = OleCreatePropertyFrame(hwndOwner, 0, 0, caption, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            // Release COM objects
            Marshal.FreeCoTaskMem(caGUID.pElems);
            Marshal.ReleaseComObject(pProp);
        }

        /// <summary>
        /// Displays property page for device.
        /// </summary>
        /// <param name="moniker">Moniker (device identification) of camera.</param>
        /// <param name="hwndOwner">The window handler for to make it parent of property page.</param>
        /// <seealso cref="Moniker"/>
        public static void DisplayPropertyPage(IMoniker moniker, IntPtr hwndOwner)
        {
            if (moniker == null)
                return;

            object source = null;
            Guid iid = typeof(IBaseFilter).GUID;
            moniker.BindToObject(null, null, ref iid, out source);
            IBaseFilter theDevice = (IBaseFilter)source;

            _DisplayPropertyPage(theDevice, hwndOwner);

            //Release COM objects
            Marshal.ReleaseComObject(theDevice);
            theDevice = null;
        }

        #endregion
    }
}
