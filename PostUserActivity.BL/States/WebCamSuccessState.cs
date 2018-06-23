using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class WebCamSuccessState : WebCamStateBase
    {
        #region ctor

        private WebCamSuccessState()
        {
            this.State = ProcessStateType.Success;
        }

        public WebCamSuccessState(MainWindowController controller):this()
        {
        }

        #endregion

        #region Overrides of DeviceStateBase

        public override DeviceStateBase ChangeState()
        {
            return this;
        }

        #endregion
    }
}
