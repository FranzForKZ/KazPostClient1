using System;
using CommonLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostUserActivity.HW;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class SystemInformation_UnitTest
    {

        /// <summary>
        /// это не автоматический тест, значение сравшинвается с реальным UUID, который получен wmic path win32_computersystemproduct get uuid
        /// </summary>
        [TestMethod]
        public void GetUUID_TestMethod()
        {
            var uuid = SystemInfo.GetUUID();
            var myUUID = "BF9D84E8-45A1-5243-0033-3497F689086B";            
            Assert.AreEqual(uuid, myUUID);
        }

        [TestMethod]
        public void HashFunction_TestMethod()
        {
            var date = new DateTime(2016,11,20,19,0,0);
            var hash = HashFunction.Get("uuid", "secretWord", date, "login");
        }


        [TestMethod]
        public void GetCurrentUserDomainName_TestMethod()
        {
            var userName = SystemInfo.GetCurrentUserDomainName();

            Assert.IsFalse(string.IsNullOrEmpty(userName));
            Assert.AreEqual("AD\\abelov", userName);
        }
    }
}
