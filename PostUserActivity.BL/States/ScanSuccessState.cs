using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class ScanSuccessState : ScannerStateBase
    {
        #region ctor

        private ScanSuccessState()
        {
            this.State = ProcessStateType.Success;
        }

        public ScanSuccessState(MainWindowController controller):this()
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
