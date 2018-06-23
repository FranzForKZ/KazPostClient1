using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class ScanErrorState : ScannerStateBase
    {
        #region ctor
        private MainWindowController controller;
        private ScanErrorState()
        {
            this.State = ProcessStateType.Error;
        }

        public ScanErrorState(MainWindowController controller):this()
        {
            this.controller = controller;
        }

        #endregion

        #region Overrides of DeviceStateBase

        public override DeviceStateBase ChangeState()
        {
             System.Configuration.ConfigurationManager.RefreshSection("appSettings");
             string LoadPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ScanJpgPath"]) ? "" : System.Configuration.ConfigurationManager.AppSettings["ScanJpgPath"];
            if (LoadPath != string.Empty)
            {
                System.IO.DirectoryInfo DI = new System.IO.DirectoryInfo(LoadPath);
                if (DI.GetFiles("*.jpg").Count() != 0 && controller.ScannerAnalyzeResult != null)
                {
                    var state = new ScanSuccessState(controller);
                    state.ErrorsList = ErrorsList;
                    state.ErrorMessage = ErrorMessage;
                    return state;
                }
                else
                {
                    var HDDstateError = new ScanErrorState(controller);
                    HDDstateError.ErrorsList = new List<Contracts.AnalyzeImageResultType>();
                    HDDstateError.ErrorsList.AddRange(ErrorsList);
                    HDDstateError.ErrorMessage = ErrorMessage;
                    return HDDstateError;
                }
            }
            return this;
        }

        #endregion
    }
}
