using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.HW.WIALib
{
    public class WiaDevice
    {
        #region Properties

        /// <summary>
        /// The index of the device in the WiaManager.
        /// </summary>
        public int ManagerIndex { get; private set; }

        /// <summary>
        /// The Globally-unique identifier 
        /// </summary>
        public string ManagerGuid { get; private set; }

        /// <summary>
        /// The system name of the device.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The manufacturer of the device.
        /// </summary>
        public string Manufacturer { get; private set; }

        /// <summary>
        /// The type of device.
        /// </summary>
        public WIADeviceTypes Type { get; private set; }

        #endregion

        /// <summary>
        /// The internal constructor.
        /// </summary>
        /// <param name="index">The index of the device in the InteropManager's Array</param>
        /// <param name="device">The WIA DeviceInfo object for the device.</param>
        internal WiaDevice(int index, WIA.DeviceInfo device)
        {
            this.ManagerIndex = index;
            this.ManagerGuid = device.DeviceID;
            this.Name = (string)device.Properties.GetProperty(7);
            this.Manufacturer = (string)device.Properties.GetProperty(3);
            this.Type = (WIADeviceTypes)(int)(device.Properties.GetProperty(5));
        }

    }
}
