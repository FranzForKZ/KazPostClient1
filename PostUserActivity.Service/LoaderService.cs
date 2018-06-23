using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PostUserActivity.Forms;
using System.Windows.Forms;
using CommonLib;


namespace PostUserActivity.Service
{
    public class LoaderService
    {
        //WinFormStarter<ScannersSettings> scannerSettingsForm = new WinFormStarter<ScannersSettings>();
        //WinFormStarter<WebCamSettings> webCamSettingsForm = new WinFormStarter<WebCamSettings>();

        internal void Start()
        {
            // the name of the application to launch
            String applicationName = "cmd.exe";

            //scannerSettingsForm.Show();
            
            //RegistryKey ckey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\", true);
            //SetInterActWithDeskTop();
            //sc.ShowDialog();
            //Application.Run(new ScannersSettings());
            //throw new NotImplementedException();
        }


        internal bool Stop()
        {
            //throw new NotImplementedException();
            return true;
        }

        private static void SetInterActWithDeskTop()
        {
            var service = new System.Management.ManagementObject(string.Format("WIN32_Service.Name='{0}'", "DataCollector.JobService"));
            try
            {
                //var paramList = new object[11];
                //paramList[5] = true;                
                //service.InvokeMethod("Change", paramList);
                var desktopInteractProp = getPropertiesList(service.Properties).
                    First(p => p.Name.ToLowerInvariant() == "DesktopInteract".ToLowerInvariant());
                //desktopInteractProp.Value = true;
                //var servMethodParams = service.GetMethodParameters("Change");
                //var desktopInteractProp = getPropertiesList(servMethodParams.Properties)
                //    .First(p => p.Name.ToLowerInvariant() == "DesktopInteract".ToLowerInvariant());
                desktopInteractProp.Value = true;

                //service.InvokeMethod("Change", servMethodParams, null);
                //service.InvokeMethod("Change", servMethodParams.Properties);
            }
            finally
            {
                service.Dispose();
            }
        }

        private static IEnumerable<System.Management.PropertyData> getPropertiesList(System.Management.PropertyDataCollection propertyCollection)
        {
            foreach (var p in propertyCollection)
            {
                yield return p;
            }
        }
    }
}
