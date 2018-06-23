using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class ScanInProgressState : ScannerStateBase
    {
        private MainWindowController controler;

        #region ctor

        private ScanInProgressState()
        {
            this.State = ProcessStateType.InProgress;
        }

        public ScanInProgressState(MainWindowController controler):this()
        {
            this.controler = controler;
        }

        #endregion

        #region Overrides of DeviceStateBase

        public override DeviceStateBase ChangeState()
        {
            if (controler.ScannerAnalyzeResult != null && controler.ScannerAnalyzeResult.IsSuccess())
            {
                var state = new ScanSuccessState(controler);
                state.ErrorsList = ErrorsList;
                state.ErrorMessage = ErrorMessage;
                return state;
            }


            if (controler.CanScan())
            {
                var state  = new ScanErrorState(controler);
                state.ErrorsList = ErrorsList;
                state.ErrorMessage = ErrorMessage;
                return state;
            }
            else            
            {
                var state = new ScanExceededAttemptsState(controler);
                state.ErrorsList = ErrorsList;
                state.ErrorMessage = ErrorMessage;
                return state;
            }
        }

        #endregion
    }
}
