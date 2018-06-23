using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Contracts.Config
{
    /// <summary>
    /// общий интерфес для более удобной работы с конфигом железок
    /// </summary>
    public interface IDeviceConfiguration
    {
        HWDeviceDesciption GetDevice(DeviceType deviceType);
        void SaveDevice(HWDeviceDesciption device);

        void Reload();        
    }
}
