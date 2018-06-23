using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.Forms;

namespace PostUserActivity.Contracts.Forms
{
    public class GetWebCamImagePictureDialog : FormPropertiesBase, IPictureDialog
    {
        public GetWebCamImagePictureDialog(IDeviceConfiguration config) : base(config, DeviceType.WebCam)
        {
        }

        #region Implementation of IPictureDialog

        public string GetHeaderText()
        {
            return "Запись видео";
        }

        public string GetButtonText(int retryCount)
        {
            return retryCount == 0 ? "Начать запись" : "Записать повторно";
        }

        public string GetEmptyConfigErrorText()
        {
            return "В настройках приложения не найдена камера, с которой будет происходить запись";
        }

        #endregion
    }
}
