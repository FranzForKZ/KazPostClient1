using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Contracts.Forms
{
    public abstract class DeviceStateBase
    {
        public ProcessStateType State { get; protected set; }

        public virtual event EventHandler StateChanged = (sender, args) => { };

        public abstract DeviceStateBase ChangeState();

        public DeviceType Device { get; protected set; }


        public List<AnalyzeImageResultType> ErrorsList { get; set; }

        public string ErrorMessage { get; set; }
    }
    public abstract class WebCamStateBase: DeviceStateBase
    {
        public WebCamStateBase()
        {
            Device = DeviceType.WebCam;
        }
    }

    public abstract class ScannerStateBase : DeviceStateBase
    {
        public ScannerStateBase()
        {
            Device = DeviceType.Scanner;
        }
    }
}
