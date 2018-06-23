using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts.Network
{
    public enum RequestResultType
    {
        ConnectionUnavailable = 0,
        Successful = 200,
        Created = 201,
        Accepted = 202,
        BadRequest = 400,
        
        UnAuthrized = 401,
        Payment_Required = 402,
        RequestedHostUnavailable = 434,
        ERROR = 500,
        ServerUnavailable = 503,
        ViolationLicenses = 500,
        
        ViolationNumberLicenses = 7383,
        AppNotRegistered = 7384
    }
}
