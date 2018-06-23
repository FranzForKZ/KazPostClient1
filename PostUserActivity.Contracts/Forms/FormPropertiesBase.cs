using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Forms
{
    public abstract class FormPropertiesBase : ISettingsDeviceDialog
    {
        public FormPropertiesBase(IDeviceConfiguration config, DeviceType deviceType)
        {
            CurrentConfiguration = config;
            DeviceType = deviceType;
        }
        
        protected IDeviceConfiguration CurrentConfiguration { get; set; }
        protected DeviceType DeviceType { get; set; }

        #region Implementation of ISettingsDeviceDialog

        public IDeviceConfiguration GetDeviceConfiguration()
        {
            return CurrentConfiguration;
        }

        public DeviceType GetDeviceType()
        {
            return DeviceType;
        }

        #endregion        
    }
}
