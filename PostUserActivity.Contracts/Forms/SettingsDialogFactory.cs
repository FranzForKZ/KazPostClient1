using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.Forms;

namespace PostUserActivity.Contracts.Forms
{
    public class SettingsDialogFactory
    {
        private IDeviceConfiguration Config;

        public SettingsDialogFactory(IDeviceConfiguration config)
        {            
            this.Config = config;
        }

        public ISettingsDialog GetSettingsDialog(DeviceType device)
        {
            switch (device)
            {
                case DeviceType.Scanner:
                    return new ScannerFormProperties(Config);
                case DeviceType.WebCam:
                    return new WebCamFormProperties(Config);
            }
            throw new Exception(string.Format( "Can't find device dialog settings for this device type: {0}", device));
        }
    }
}
