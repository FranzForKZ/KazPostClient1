using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.HW.WIALib
{
    public class WiaException : ApplicationException
    {

        public WiaErrorCodes WiaErrorCode { get; private set; }

        public WiaException()
            : base()
        {

        }

        public WiaException(string message)
            : base(message)
        {

        }

        public WiaException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public WiaException(string message, int wiaErrorCode, Exception innerException)
            : base(message, innerException)
        {
            this.WiaErrorCode = (WiaErrorCodes)wiaErrorCode;
        }


    }
}
