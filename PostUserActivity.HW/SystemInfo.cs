using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Text;

namespace PostUserActivity.HW
{
    public class SystemInfo
    {
        public static string  GetUUID()
        {
            string uuid = string.Empty;
            ManagementScope Scope;
            Scope = new ManagementScope("\\\\localhost\\root\\CIMV2", null);
            Scope.Connect();
            ObjectQuery Query = new ObjectQuery("SELECT UUID FROM Win32_ComputerSystemProduct");
            ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Scope, Query);

            var wmiOject = Searcher.Get().Cast<ManagementObject>().ToList().FirstOrDefault();

            if (wmiOject != null)
            {
                uuid = wmiOject["UUID"].ToString();
            }               
            return uuid;
        }


        public static string GetCurrentUserDomainName()
        {
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();

            return currentIdentity.Name;
        }
    }
}
