using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.Forms;

namespace PostUserActivity.Contracts.Forms
{
    public class GetScanerImagePictureDialog : FormPropertiesBase, IPictureDialog
    {
        public GetScanerImagePictureDialog(IDeviceConfiguration config) : base(config, DeviceType.Scanner)
        {
        }

        #region Implementation of IPictureDialog

        public string GetHeaderText()
        {
            return "Сканирование изображения";
        }

        public string GetButtonText(int retryCount)
        {
            return retryCount == 0 ? "Сканировать" : "Сканировать повторно";
        }

        public string GetEmptyConfigErrorText()
        {
            return "В настройках приложения не найден сканер, с помощью которого будет происходить сканирование";
        }

        #endregion
    }
}
