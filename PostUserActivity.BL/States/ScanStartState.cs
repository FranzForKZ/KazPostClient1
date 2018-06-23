using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.HW;

namespace PostUserActivity.BL.States
{
    public class ScanStartState : ScannerStateBase
    {
        private MainWindowController controller;
        private bool next = false;

        #region ctor

        private ScanStartState()
        {
            this.State = ProcessStateType.Start;
        }

        public ScanStartState(MainWindowController controller): this()
        {
            this.controller = controller;
        }

        public ScanStartState(MainWindowController controller, bool next) : this()
        {
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
                        return new ScanInProgressState(controller);
                    }
                    else
                    {
                        return this;
                    }                    
                }
                var PathForImages = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ScanJpgPath"]) ? "" : System.Configuration.ConfigurationManager.AppSettings["ScanJpgPath"];
                if (PathForImages != string.Empty)
                {
                    System.IO.DirectoryInfo DI = new System.IO.DirectoryInfo(PathForImages);
                    if (DI.GetFiles("*.jpg").Count() != 0)
                    {
                        if (next)
                        {
                            return new ScanInProgressState(controller);
                        }
                        else
                        {
                            return this;
                        }
                    }

                }
            }

            return new ScanErrorState(controller);
        }

        #endregion
    }
}
