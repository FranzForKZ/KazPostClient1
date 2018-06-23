using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.HW.WIALib
{

    /// <summary>
    /// 
    /// </summary>
    public enum WiaErrorCodes
    {
        /// <summary>
        /// General error
        /// </summary>
        GeneralError = 1,

        /// <summary>
        /// Paper jam
        /// </summary>
        PaperJam = 2,

        /// <summary>
        /// Paper empty
        /// </summary>
        PaperEmpty = 3,

        /// <summary>
        /// Paper problem
        /// </summary>
        PaperProblem = 4,

        /// <summary>
        /// Offline
        /// </summary>
        Offline = 5,

        /// <summary>
        /// Busy
        /// </summary>
        Busy = 6,

        /// <summary>
        /// Warming up
        /// </summary>
        WarmingUp = 7,

        /// <summary>
        /// User intervention
        /// </summary>
        UserIntervention = 8,

        /// <summary>
        /// Item deleted
        /// </summary>
        ItemDeleted = 9,

        /// <summary>
        /// Device communication failure
        /// </summary>
        DeviceCommunication = 10,

        /// <summary>
        /// Invalid command
        /// </summary>
        InvalidCommand = 11,

        /// <summary>
        /// Incorrect hardware setting
        /// </summary>
        IncorrectHardwareSetting = 12,

        /// <summary>
        /// Device locked
        /// </summary>
        DeviceLocked = 13,

        /// <summary>
        /// Driver threw an exception
        /// </summary>
        ExceptionInDriver = 14,

        /// <summary>
        /// Invresponsealid driver 
        /// </summary>
        InvalidDriverResponse = 15,

        /// <summary>
        /// Cover open
        /// </summary>
        CoverOpen = 16,

        /// <summary>
        /// Lamp off
        /// </summary>
        LampOff = 17,

        /// <summary>
        /// Destination
        /// </summary>
        Destination = 18,

        /// <summary>
        /// Network reservation failed
        /// </summary>
        NetworkReservationFailed = 19
    }
}
