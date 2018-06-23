using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.HW.WIALib
{
    /// <summary>
    /// Describes the different types of DocumentSources available to scanners.
    /// </summary>
    public enum DocumentSources
    {

        /// <summary>
        /// Represents a one-sided sheetfed scanner.
        /// </summary>
        SingleSided = 1,

        /// <summary>
        /// Represents a one-sided flatbed scanner.
        /// </summary>
        Flatbed = 2,

        /// <summary>
        /// Represents a duplex sheet-fed scanner.
        /// </summary>
        DoubleSided = 4
    }
}
