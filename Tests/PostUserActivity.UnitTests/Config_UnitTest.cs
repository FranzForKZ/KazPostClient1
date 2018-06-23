using System;
using CommonLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.Contracts.Network;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class Config_UnitTest
    {
        [TestMethod]
        public void CreateEmptyCnfig_TestMethod()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            logger.Info("fgdsagfsajk");

            var qwe = new ConfigurationSettings();

            var emptyConfig = new ConfigurationSchema();

            emptyConfig.Scanner = new ScannerSetings()
            {
                Device = new HWDeviceDesciption()
                {
                    Name = "scanner name",
                    Device = DeviceType.Scanner,
                    DeviceId = "scanner id"
                }
            };
            emptyConfig.WebCam = new WebCamSettings()
            {
                Device = new HWDeviceDesciption()
                {
                    Device = DeviceType.WebCam,
                    DeviceId = "camera id",
                    Name = "camera name"
                }
            };


            qwe.SaveConfiguration(emptyConfig);
        }

        [TestMethod]
        public void SaveDevice_TestMethod()
        {
            IDeviceConfiguration configuration = new ConfigurationSettings();

            var webCam = new HWDeviceDesciption()
            {
                Device = DeviceType.WebCam,
                DeviceId = "camera id custom",
                Name = "camera name custom"
            };
            configuration.SaveDevice(webCam);

            var savedWebCam = configuration.GetDevice(DeviceType.WebCam);

            Assert.AreEqual(savedWebCam.Device, webCam.Device);
            Assert.AreEqual(savedWebCam.DeviceId, webCam.DeviceId);
            Assert.AreEqual(savedWebCam.Name, webCam.Name);
            Assert.AreEqual(savedWebCam.IsSet, webCam.IsSet);
        }

        [TestMethod]
        public void ArmDataPackageSerializeTypeToLowerCase_TestMethod()
        {
            var qwe  = new ArmDataPackage()
            {
                Type = ArmDataPackageType.FullFrame,
                Timestamp = DateTime.Now
            };

            var str = Newtonsoft.Json.JsonConvert.SerializeObject(qwe);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ArmDataPackage>(str);
        }

        [TestMethod]
        public void jsonTest_TestMethod()
        {
            var str1 = "slash\\";
            var str2 = "slash/";
            var str3 = "aaa'";
            var str4 = "aaa\"";
            var json1 = Newtonsoft.Json.JsonConvert.SerializeObject(str1);
            var json2 = Newtonsoft.Json.JsonConvert.SerializeObject(str2);
            var json3 = Newtonsoft.Json.JsonConvert.SerializeObject(str3);
            var json4 = Newtonsoft.Json.JsonConvert.SerializeObject(str4);
        }
    }
}
