using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.HW;

namespace PostUserActivity.BL.States
{
    public class WebCamStartState : WebCamStateBase
    {
        private MainWindowController controller;
        private bool next = false;

        #region ctor

        private WebCamStartState()
        {
            this.State = ProcessStateType.Start;
        }

        public WebCamStartState(MainWindowController controller) : this()
        {
            //  проверим, есть ли камера
            this.controller = controller;
            next = false;
        }

        public WebCamStartState(MainWindowController controller, bool next) : this()
        {
            //  проверим, есть ли камера
            this.controller = controller;
            this.next = next;
        }

        #endregion



        #region Overrides of DeviceStateBase

        public override DeviceStateBase ChangeState()
        {
            var config = this.controller.GetConfiguration();
            var device = config.GetDevice(Device);
            if (config != null && device != null)
            {
                var devicesSearcher = new DeviceSearchFactory(Device);
                var devices = devicesSearcher.GetDevices().ToList();
                if (devices.Exists(p => p.Equals(device)))
                {
                    if (next)
                    {
                        return new WebCamInProgressState(controller);
                    }
                    else
                    {
                        return this;
                    }                    
                }
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                var PathForImages = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"]) ? "" : System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"];
                if (PathForImages != string.Empty)
                {
                    System.IO.DirectoryInfo DI = new System.IO.DirectoryInfo(PathForImages);
                    if (DI.GetFiles("*.jpg").Count() != 0)
                    {
                        if (next)
                        {
                            return new WebCamInProgressState(controller);
                        }
                        else
                        {
                            return this;
                        }
                    }

                }
            }

            return new WebCamErrorState(controller);
        }

        #endregion
    }
}
