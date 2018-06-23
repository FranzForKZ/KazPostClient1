﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Forms;

namespace PostUserActivity.BL.States
{
    public class WebCamInProgressState : WebCamStateBase
    {
        private MainWindowController controler;

        #region ctor

        private WebCamInProgressState()
        {
            this.State = ProcessStateType.InProgress;
        }

        public WebCamInProgressState(MainWindowController controler) : this()
        {
            this.controler = controler;
        }

        #endregion

        #region Overrides of DeviceStateBase

        public override DeviceStateBase ChangeState()
        {
            if (controler.WebCamAnalyzeResult != null && controler.WebCamAnalyzeResult.IsSuccess())
            {
                var state = new WebCamSuccessState(controler);
                state.ErrorsList = ErrorsList;
                state.ErrorMessage = ErrorMessage;
                return state;
            }


            if (controler.CanCam())
            {
                var state = new WebCamErrorState(controler);
                state.ErrorsList.AddRange(ErrorsList);
                state.ErrorsList = state.ErrorsList.Distinct().ToList();
                state.ErrorMessage = ErrorMessage;
                return state;
            }
            else
            {
                var state = new WebCamExceededAttemptsState(controler);
                state.ErrorsList.AddRange(ErrorsList);
                state.ErrorsList = state.ErrorsList.Distinct().ToList();
                state.ErrorMessage = ErrorMessage;
                return state;
            }
        }

        #endregion
    }
}
