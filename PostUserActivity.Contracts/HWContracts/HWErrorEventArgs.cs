using System;

namespace PostUserActivity.Contracts.HWContracts
{
    public class HWErrorEventArgs : EventArgs
    {
        public HWErrorEventArgs(string msg)
        {
            this.Message = msg;
        }
        public string Message { get; private set; }
    }
}
