using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts.HWContracts
{
    public class HWCompletedEventArgsEventArgs : EventArgs
    {        
        public HWCompletedEventArgsEventArgs(string msg)
        {
            Message = msg;
        }
        
        public HWCompletedEventArgsEventArgs(Image img, string msg)
        {
            Image = img;
            Message = msg;
        }
        public HWCompletedEventArgsEventArgs(Image img, string msg, bool isRecordStopped)
        {
            Image = img;
            Message = msg;
            IsRecordStopped = isRecordStopped;
        }

        public bool IsRecordStopped { get; private set; }


        public Image Image { get; private set; }
        public string Message { get; private set; }        
    }
}
