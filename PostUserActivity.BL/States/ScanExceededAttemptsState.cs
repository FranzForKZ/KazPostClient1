using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class ScanExceededAttemptsState: ScannerStateBase
    {
        #region ctor
        
        private ScanExceededAttemptsState()
        {
            this.State = ProcessStateType.ExceededAttempts;
        }

        public ScanExceededAttemptsState(MainWindowController controller):this()
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
