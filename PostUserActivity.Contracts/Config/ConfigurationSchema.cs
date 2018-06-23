using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.Contracts.Network;

namespace PostUserActivity.Contracts.Config
{
    /// <summary>
    /// общие настройки сканера
    /// </summary>
    public class ScannerSetings
    {
        public ScannerSetings()
        {
            Device = new HWDeviceDesciption();
        }

        public HWDeviceDesciption Device { get; set; }
        public int Resolution { get; set; }
        public string PixelType { get; set; }        
    }

    /// <summary>
    /// общие настройки камеры
    /// </summary>
    public class WebCamSettings
    {
        public WebCamSettings()
        {
            Device = new HWDeviceDesciption();            
        }

        public HWDeviceDesciption Device { get; set; }
        public int ResolutionHeight { get; set; }
        public int ResolutionWidth { get; set; }        
    }

    /// <summary>
    /// общая структура для чтения или сохранения настроек железок
    /// </summary>
    public class ConfigurationSchema
    {
        public ConfigurationSchema()
        {
            Scanner = new ScannerSetings();
            WebCam = new WebCamSettings();
            ImagesOutputFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaFolder");
            ToUploadFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
            SendedFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sended");
            UUID = string.Empty;
            ArmRegistered = "false";
        }

        public string ArmRegistered { get; set; }
        public string SendedFolder { get; set; }
        public string ToUploadFolder { get; set; }

        public string UUID { get; set; }
        public string ImagesOutputFolder { get; set; }

        public ScannerSetings Scanner { get; set; }
        public WebCamSettings WebCam { get; set; }
        
        public ArmSettingsParams ArmParams { get; set; }
    }
}
