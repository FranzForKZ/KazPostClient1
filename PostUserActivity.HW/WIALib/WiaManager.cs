using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using WIA;

namespace PostUserActivity.HW.WIALib
{
    public class WiaManager
    {

        #region Constants

        private const int WIA_PROPERTIES_WIA_RESERVED_FOR_NEW_PROPS = 1024;
        private const int WIA_PROPERTIES_WIA_DIP_FIRST = 2;

        private const int WIA_PROPERTIES_WIA_DPA_FIRST = WIA_PROPERTIES_WIA_DIP_FIRST + WIA_PROPERTIES_WIA_RESERVED_FOR_NEW_PROPS;
        private const int WIA_PROPERTIES_WIA_DPC_FIRST = WIA_PROPERTIES_WIA_DPA_FIRST + WIA_PROPERTIES_WIA_RESERVED_FOR_NEW_PROPS;
        private const int WIA_PROPERTIES_WIA_DPS_FIRST = WIA_PROPERTIES_WIA_DPC_FIRST + WIA_PROPERTIES_WIA_RESERVED_FOR_NEW_PROPS;

        private const int WIA_PROPERTIES_WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_PROPERTIES_WIA_DPS_FIRST + 13;
        private const int WIA_PROPERTIES_WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_PROPERTIES_WIA_DPS_FIRST + 14;
        private const int WIA_IPS_CUR_INTENT = 6146;
        private const int WIA_IPS_XRES = 6147;
        private const int WIA_IPS_YRES = 6148;
        private const int FEED_READY = 0x01;

        #endregion

        #region Private Members

        private readonly DeviceManager _deviceManager;

        #endregion

        #region Properties

        /// <summary>
        /// A WPF-bindable list of the WiaDevices currently attached to the system.
        /// </summary>
        public ObservableCollection<WiaDevice> Devices { get; private set; }

        #endregion

        #region Events

        public event EventHandler<EventArgs> ItemAcquired;
        public event EventHandler<EventArgs> AcquisitionCompleted;

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public WiaManager()
        {
            _deviceManager = new DeviceManager();
            this.Devices = new ObservableCollection<WiaDevice>();
            var count = _deviceManager.DeviceInfos.Count;

            for (var i = 1; i < (count + 1); i++)
            {
                this.Devices.Add(new WiaDevice(i, (DeviceInfo) _deviceManager.DeviceInfos[i]));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wiaDevice"></param>
        /// <param name="source"></param>
        /// <param name="scanType"></param>
        /// <param name="quality"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public WiaResult AcquireScan(WiaDevice wiaDevice, DocumentSources source, ScanTypes scanType,
            ScanQualityTypes quality = ScanQualityTypes.None, int resolution = 150)
        {
            if (wiaDevice.Type != WIADeviceTypes.Scanner) throw new Exception("The selected device is not a scanner.");

            var interim = new List<byte[]>();
            try
            {
                object index = wiaDevice.ManagerIndex;
                DeviceInfo deviceInfo = _deviceManager.DeviceInfos[index];
                Device device = deviceInfo.Connect();

                int intentValue = (int) scanType + (int) quality;
                int documentStatus = 0;

                device.Properties.SetProperty(WIA_PROPERTIES_WIA_DPS_DOCUMENT_HANDLING_SELECT, source);
                device.Properties.SetProperty(WIA_IPS_XRES, resolution);
                device.Properties.SetProperty(WIA_IPS_YRES, resolution);
                device.Properties.SetProperty(WIA_IPS_CUR_INTENT, intentValue);
                documentStatus = (int) device.Properties.GetProperty(WIA_PROPERTIES_WIA_DPS_DOCUMENT_HANDLING_STATUS);

                foreach (Item itm in device.Items)
                {
                    while ((documentStatus & FEED_READY) == FEED_READY)
                    {
                        //Get item flags
                        var flag = (WiaItemFlag) itm.Properties.GetProperty(4101);

                        //This process can only handle images. Everything else is to be ignored.
                        if ((flag & WiaItemFlag.ImageItemFlag) != WiaItemFlag.ImageItemFlag)
                            continue;
                        ImageFile image = (ImageFile) itm.Transfer();
                        documentStatus = (int) device.Properties.GetProperty(WIA_PROPERTIES_WIA_DPS_DOCUMENT_HANDLING_STATUS);
                        var bytes = image.ToByte();
                        interim.Add(bytes);

                        if (this.ItemAcquired != null)
                            this.ItemAcquired(this, new EventArgs());

                    }
                }

            }
            catch (COMException cx)
            {
                var code = cx.GetWiaErrorCode();
                if (code != 3)
                    throw new WiaException(WiaExtensions.GetErrorCodeDescription(code), code, cx);
            }

            if (this.AcquisitionCompleted != null)
                this.AcquisitionCompleted(this, new EventArgs());

            var result = new WiaResult(interim);
            return result;
        }

        #endregion

    }
}
