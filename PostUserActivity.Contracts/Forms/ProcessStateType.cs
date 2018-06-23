using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts.Forms
{
    public enum ProcessStateType
    {
        Start,
        Error,
        Success,
        InProgress,
        ExceededAttempts
    }
}
