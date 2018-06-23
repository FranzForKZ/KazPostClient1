using System;
using System.Collections.Generic;
using System.Drawing;
using CommonLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostUserActivity.Contracts.Network;
using PostUserActivity.HW;
using PostUserActivity.Net;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class Net_UnitTest
    {
        [TestMethod]
        public void Auth_TestMethod()
        {
            var req = new NetRequests();
            var userName = SystemInfo.GetCurrentUserDomainName();
            var hash = HashFunction.Get("SecretWord");
            var result = req.SetAuthRequest(userName, hash);
        }

        [TestMethod]
        public void RegisterNewARM_TestMethod()
        {
            var userName = SystemInfo.GetCurrentUserDomainName();
            var uuid = SystemInfo.GetUUID();
            var req = new NetRequests();
            var ressult = req.RegisterArm(uuid, userName, "RegisterNewARM_TestMethod");
            
            Assert.AreEqual(ressult.RequestResult, RequestResultType.Successful);
        }

        [TestMethod]
        public void RegisterAndGetAuth_TestMethod()
        {
            var userName = SystemInfo.GetCurrentUserDomainName();
            var uuid = SystemInfo.GetUUID();
            var req = new NetRequests();
            var resultReg = req.RegisterArm(uuid, userName, "RegisterNewARM_TestMethod");

            var settings = new ConfigurationSettings();
            settings.SaveUUID(uuid);

            //settings.SaveArmGUID(Guid.NewGuid().ToString());

            var hash = HashFunction.Get("SecretWord");
            var resultAuth = req.SetAuthRequest(userName, hash);
            if (resultAuth.RequestResult == RequestResultType.Successful)
            {

                settings.SaveArmSettings(resultAuth.Result);
            }

            Assert.AreEqual(resultReg.RequestResult, RequestResultType.Successful);
            Assert.AreEqual(resultAuth.RequestResult, RequestResultType.Successful);
            Assert.IsFalse(resultAuth.Result == null);
        }

        [TestMethod]
        public void SendPackage_TestMethod()
        {
            var userName = SystemInfo.GetCurrentUserDomainName();
            var uuid = SystemInfo.GetUUID();
            var req = new NetRequests();
            var settings = new ConfigurationSettings();

            var hash = HashFunction.Get("SecretWord");

            var photoImg = Image.FromFile("TestImages\\goodPhotoTest.jpg");
            var scanImg = Image.FromFile("TestImages\\scanTest.jpg");

            var data = new ArmDataPackage()
            {
                Type = ArmDataPackageType.Preview,
                IIN = 13543543L,
                Timestamp = DateTime.Now,
                Comment = "test",
                Token = hash,
                UserName = userName,
                WFMId = 56789L,                
                CameraPicture = photoImg.ToBase64(),
                ScanPicture = scanImg.ToBase64()
            };
            var sendResult = req.SendPackage(data);

            Assert.IsTrue(sendResult.RequestResult == RequestResultType.Successful);
            //}
        }

        [TestMethod]
        public void NetSender_Send_TestMethodAttribute()
        {
            var settings = new ConfigurationSettings();
            var sender = new NetSender(settings);

            var hash = HashFunction.Get("SecretWord");
            var userName = SystemInfo.GetCurrentUserDomainName();

            var photoImg = Image.FromFile("TestImages\\goodPhotoTest.jpg");
            var scanImg = Image.FromFile("TestImages\\scanTest.jpg");

            var dataPreview = new ArmDataPackage()
            {
                Type = ArmDataPackageType.Preview,
                IIN = 13543543L,
                Timestamp = DateTime.Now,
                Comment = "test",
                Token = hash,
                UserName = userName,
                WFMId = 56789L,
                CameraPicture = photoImg.ToBase64(),
                ScanPicture = scanImg.ToBase64()
            };

            var dataFull = new ArmDataPackage()
            {
                Type = ArmDataPackageType.FullFrame,
                IIN = 13543543L,
                Timestamp = DateTime.Now,
                Comment = "test",
                Token = hash,
                UserName = userName,
                WFMId = 56789L,
                CameraPicture = photoImg.ToBase64(),
                ScanPicture = scanImg.ToBase64()
            };

            var packets = new List<ArmDataPackage>();
            packets.Add(dataFull);
            packets.Add(dataPreview);

            sender.Send(packets);
        }
    }
}
