using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts.Forms
{
    public class FormWorkResult
    {
        public bool IsSuccessful { get; set; }
        public string FolderName { get; set; }
        public Image[] Images { get; set; }
    }
}
