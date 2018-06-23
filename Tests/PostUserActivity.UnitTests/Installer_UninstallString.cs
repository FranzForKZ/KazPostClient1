using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System.Management;
namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class Installer_UninstallString
    {
        [TestMethod]
        public void UninstallString()
        {
            
            
                string query = string.Format("select * from Win32_Product where Name='{0}'", "KazPostArm");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    
                    var tempValue = searcher.Get();
                    foreach (ManagementObject product in searcher.Get())
                    {
                        Console.WriteLine("msiexec.exe /x " + product["IdentifyingNumber"].ToString());
                        return;
                    }
                    
                }
                return;
               // Console.WriteLine(uninstallString);
        }
        [TestMethod]
        public void UninstallString1()
        {
           
            string uninstallString = string.Empty;
            try
            {
                string path = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Installer\\UserData\\S-1-5-18\\Products";

                RegistryKey key = Registry.LocalMachine.OpenSubKey(path);
                foreach (string tempKeyName in key.GetSubKeyNames())
                {
                    RegistryKey tempKey = key.OpenSubKey(tempKeyName + "\\InstallProperties");
                    if (tempKey != null)
                    {
                        //if (string.Equals(Convert.ToString(tempKey.GetValue("DisplayName")), msiName, StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    uninstallString = Convert.ToString(tempKey.GetValue("UninstallString"));
                        //    uninstallString = uninstallString.Replace("/I", "/X");
                        //    uninstallString = uninstallString.Replace("MsiExec.exe", "").Trim();
                        //    uninstallString += " /quiet /qn";
                        //    break;
                        //}
                    }
                }

               // return uninstallString;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
