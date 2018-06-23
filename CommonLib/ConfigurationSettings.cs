using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.Contracts.Network;
using YamlDotNet.Serialization;

namespace CommonLib
{
    /// <summary>
    /// чтение и сохранение настроек железок, настройки сохраняются в папку пользователя (%UserProfile%\AppData\Roaming\PostUserActivity)
    /// </summary>
    public class ConfigurationSettings : IDeviceConfiguration
    {
        private static readonly string UserSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostUserActivity");
        private static readonly string configFile = Path.Combine(UserSettingsFolder, "config.yaml");

        private static ConfigurationSchema currentConfiguration;

        private static object lockObject = new object();

        public ConfigurationSettings()
        {
            if (!File.Exists(configFile))
            {
                //  need create file                    
                var emptyConfig = new ConfigurationSchema();
                Write(emptyConfig);
            }
            Reload();
        }

        private void Write(ConfigurationSchema config)
        {
            if (!Directory.Exists(UserSettingsFolder))
            {
                Directory.CreateDirectory(UserSettingsFolder);
            }

            try
            {
                lock (config)
                {
                    var serializer = new Serializer();
                    using (var file = new System.IO.StreamWriter(configFile, false))
                    {
                        serializer.Serialize(file, config);
                    }
                }
            }
            catch (Exception ex)
            {
                //  TODO: add logger
            }
        }

        private ConfigurationSchema Read()
        {
            ConfigurationSchema config = new ConfigurationSchema();
            var deserializer = new Deserializer();
            try
            {
                lock (config)
                {
                    using (var file = new System.IO.StreamReader(configFile))
                    {
                        config = deserializer.Deserialize<ConfigurationSchema>(file);
                    }
                }
            }
            catch (Exception ex)
            {
                //  TODO: add logger
            }

            return config;
        }


        public HWDeviceDesciption GetDevice(DeviceType deviceType)
        {
            HWDeviceDesciption device = new HWDeviceDesciption();
            switch (deviceType)
            {
                case DeviceType.Scanner:
                    device = currentConfiguration.Scanner.Device;
                    break;
                case DeviceType.WebCam:
                    device = currentConfiguration.WebCam.Device;
                    break;
            }
            return device;
        }

        public void SaveDevice(HWDeviceDesciption device)
        {
            switch (device.Device)
            {
                case DeviceType.Scanner:
                    currentConfiguration.Scanner.Device = device;
                    break;
                case DeviceType.WebCam:
                    currentConfiguration.WebCam.Device = device;
                    break;
            }
            SaveConfiguration();
        }

        public void Reload()
        {
            currentConfiguration = Read();
        }
        
        public void SaveScannerSetings(ScannerSetings settings)
        {
            currentConfiguration.Scanner = settings;
            SaveConfiguration();
        }

        public void SaveSebCamSetings(WebCamSettings settings)
        {
            currentConfiguration.WebCam = settings;
            SaveConfiguration();
        }

        public void SaveConfiguration()
        {
            Write(currentConfiguration);
        }
        public void SaveConfiguration(ConfigurationSchema config)
        {
            currentConfiguration = config;
            Write(config);
        }
        public void SaveSuccessfulRegistration()
        {
            currentConfiguration.ArmRegistered = "true";
            Write(currentConfiguration);
        }
        public ConfigurationSchema GetConfiguration()
        {
            return currentConfiguration;
        }

        public string GetRegistration()
        {
            return currentConfiguration.ArmRegistered;
        }
        public string GetUUID()
        {
            return currentConfiguration.UUID;
        }

        public void SaveUUID(string uuid)
        {
            currentConfiguration.UUID = uuid;
            SaveConfiguration();
        }


        public ArmSettingsParams GetArmSettings()
        {
            return currentConfiguration.ArmParams;
        }

        public void SaveArmSettings(ArmSettingsParams armSetting)
        {
            currentConfiguration.ArmParams = armSetting;
            SaveConfiguration();
        }

        public string GetUploadPath()
        {
            try
            {
                if (!System.IO.Directory.Exists(currentConfiguration.ToUploadFolder))
                    System.IO.Directory.CreateDirectory(currentConfiguration.ToUploadFolder);
            }
            catch (Exception ex)
            {
                //  TODO: add logger
            }


            return currentConfiguration.ToUploadFolder;
        }

        public string GetSendedPath()
        {
            try
            {
                if (!System.IO.Directory.Exists(currentConfiguration.SendedFolder))
                    System.IO.Directory.CreateDirectory(currentConfiguration.SendedFolder);
            }
            catch (Exception ex)
            {
                //  TODO: add logger
            }

            return currentConfiguration.SendedFolder;
        }
    }


}
