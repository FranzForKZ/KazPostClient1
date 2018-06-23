using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.HW
{
    /// <summary>
    /// фабрика для получения конретного экземпляра класса для работы с железками
    /// </summary>
    public class DeviceWorkFactory
    {
        private ConfigurationSchema config;
        
        public DeviceWorkFactory(IDeviceConfiguration config)
        {
            this.config = ((ConfigurationSettings)config).GetConfiguration();
        }
        
        public IHWDeviceWork GetDeviceWorkFactory(DeviceType device)
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            switch (device)
            {
                case DeviceType.Scanner:
                    return new Scanner(config.Scanner.Device);
                case DeviceType.WebCam:
                    return new WebCam(config.ArmParams.VideoRecordMaxDuration, config.WebCam.Device);
                case DeviceType.HDDrive:
                    return new HDDDriveDevice(config.ArmParams.VideoRecordMaxDuration,System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"], System.Configuration.ConfigurationManager.AppSettings["ImageFromDiskPerSecond"]);
            }
            throw new Exception(string.Format("Can't find device for this device type: {0}", device));
        }
    }
}
