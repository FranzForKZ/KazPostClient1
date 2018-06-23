using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class WebCamErrorState : WebCamStateBase
    {
        private MainWindowController controller;

        #region ctor

        private WebCamErrorState()
        {
            this.State = ProcessStateType.Error;
        }

        public WebCamErrorState(MainWindowController controller):this()
        {
            this.controller = controller;
            ErrorsList = new List<AnalyzeImageResultType>();
        }

        #endregion

        #region Overrides of DeviceStateBase

        public override DeviceStateBase ChangeState()
        {
            
            if (controller.WebCamAnalyzeResult != null && controller.WebCamAnalyzeResult.IsSuccess())
            {
                var state = new WebCamSuccessState(controller);
                state.ErrorsList = ErrorsList;
                state.ErrorMessage = ErrorMessage;
                return state;
            }
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            string LoadPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"]) ? "" : System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"];
            if (LoadPath != string.Empty)
            {
                System.IO.DirectoryInfo DI = new System.IO.DirectoryInfo(LoadPath);
                if(DI.GetFiles("*.jpg").Count() != 0)
                {
                    var state = new WebCamSuccessState(controller);
                    state.ErrorsList = ErrorsList;
                    state.ErrorMessage = ErrorMessage;
                    return state;
                }
                else
                {
                    var HDDstateError = new WebCamErrorState(controller);
                    HDDstateError.ErrorsList.AddRange(ErrorsList);
                    HDDstateError.ErrorsList = HDDstateError.ErrorsList.Distinct().ToList();
                    HDDstateError.ErrorMessage = ErrorMessage;
                    return HDDstateError;
                }
                
            }

            var stateError = new WebCamErrorState(controller);
            stateError.ErrorsList.AddRange(ErrorsList);
            stateError.ErrorsList = stateError.ErrorsList.Distinct().ToList();
            stateError.ErrorMessage = ErrorMessage;
            return stateError;

            //return this;
        }

        #endregion
    }
}
