using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Forms
{
    public class ScannerFormProperties : FormPropertiesBase, ISettingsDialog
    {
        public ScannerFormProperties(IDeviceConfiguration config) : base(config, DeviceType.Scanner)
        {
        }

        #region Implementation of ISettingsDialog

        public string GetNotFoundMessage()
        {
            return "Сканеры не найдены";
        }

        public string GetWindowHeaderText()
        {
            return "Выбор установленных сканеров";
        }
    
        #endregion
    }
}
