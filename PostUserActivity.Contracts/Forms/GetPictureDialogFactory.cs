using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Contracts.Forms
{

    public interface IPictureDialog : ISettingsDeviceDialog
    {
        string GetHeaderText();
        string GetButtonText(int retryCount);
        string GetEmptyConfigErrorText();
    }

    public class GetPictureDialogFactory
    {
        private IDeviceConfiguration Config;
        public GetPictureDialogFactory(IDeviceConfiguration config)
        {
            this.Config = config;
        }

        public IPictureDialog GetPictureDialog(DeviceType device)
        {
            switch (device)
            {
                case DeviceType.Scanner:
                    return new GetScanerImagePictureDialog(Config);
                case DeviceType.WebCam:
                    return new GetWebCamImagePictureDialog(Config);
            }
            throw new Exception(string.Format("Can't find device dialog settings for this device type: {0}", device));
        }
    }
}
