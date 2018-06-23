using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CommonLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PostUserActivity.Contracts.Network;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public     class PaketImages_UnitTest
    {
        [TestMethod]
        public void ReadPaketSaveImg_TestMethod()
        {
            var filePath = @"d:\!!Projects\TechnoServ\SrcCs\PostUserActivity\bin\Debug\Upload\packet_2016-12-27 17-48-24_123465465_123456789012_FullFrame.json";

            string fileContent = "";
            FileHelper.ReadFile(filePath, out fileContent);

            ArmDataPackage package = JsonConvert.DeserializeObject<ArmDataPackage>(fileContent);

            var img = package.ScanPicture.ToImage();
            
            img.Save(@"d:\frompaket.jpg");
        }
    }
}
