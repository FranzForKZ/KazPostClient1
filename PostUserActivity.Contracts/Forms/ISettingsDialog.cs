using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Contracts.Forms
{
    /// <summary>
    /// вспомогательных интерфейс для классов описывающих настройку (выбор) конкретной железки, в зависимости от их типа
    /// </summary>
    public interface ISettingsDeviceDialog
    {
        IDeviceConfiguration GetDeviceConfiguration();
        DeviceType GetDeviceType();
    }

    /// <summary>
    /// интерфейс для классов описывающих настройку (выбор) конкретной железки, в зависимости от их типа
    /// </summary>
    public interface ISettingsDialog : ISettingsDeviceDialog
    {     
        string GetNotFoundMessage();
        string GetWindowHeaderText();        

    }
}
