using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class WebCamExceededAttemptsState: WebCamStateBase
    {
        #region ctor
        
        private WebCamExceededAttemptsState()
        {
            this.State = ProcessStateType.ExceededAttempts;
        }

        public WebCamExceededAttemptsState(MainWindowController controller):this()
        {
            ErrorsList = new List<AnalyzeImageResultType>();
        }

        #endregion


        #region Overrides of DeviceStateBase

        public override DeviceStateBase ChangeState()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
