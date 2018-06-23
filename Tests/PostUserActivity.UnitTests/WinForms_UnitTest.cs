using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostUserActivity.Forms;
using System.Windows.Forms;
using CommonLib;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Config;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class WinForms_UnitTest
    {
        /// <summary>
        /// это не автоматический тест
        /// </summary>
        [TestMethod]
        public void ShowDialogWithFindedScannersList_TestMethod()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ScannersSettings());

            IDeviceConfiguration configuration = new ConfigurationSettings();

            //var sc = new ScannersSettings(configuration) { ShowInTaskbar = true };
            //sc.Show();

            Thread.Sleep(new TimeSpan(0, 5, 0));
        }

        [TestMethod]
        public void MainWindow_TestMethod()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var config = new CommonLib.ConfigurationSettings();
            IImageAnalyzer imageAnalyzer = null;
            


            Thread.Sleep(new TimeSpan(0,10,0));            
        }

  
    }
}
