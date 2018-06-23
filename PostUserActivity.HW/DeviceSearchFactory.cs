using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.HW
{
    /// <summary>
    /// фабрика для поиска установленных железок, в зависимости от их типа
    /// </summary>
    public class DeviceSearchFactory : IHWSearcher
    {
        private IHWSearcher DeviceDearcher;
        public DeviceSearchFactory(DeviceType device)
        {
            switch (device)
            {
                case DeviceType.Scanner:
                    DeviceDearcher = new ScannersSearcher();
                    break;
                case DeviceType.WebCam:
                    DeviceDearcher = new WebCamsSearcher();
                    break;
                default:
                    throw new Exception(string.Format("Can't find device searcher for this device type: {0}", device));
                    break;                    
            }
        }

        #region Implementation of IHWSearcher

        public IEnumerable<HWDeviceDesciption> GetDevices()
        {
            return DeviceDearcher.GetDevices();
        }

        #endregion
    }
}
