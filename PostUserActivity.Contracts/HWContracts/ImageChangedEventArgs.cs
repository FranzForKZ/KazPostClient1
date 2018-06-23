using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts.HWContracts
{
    public class ImageChangedEventArgs : EventArgs
    {        
        public  bool СamStopped { get; private set; }

        public ImageChangedEventArgs(Image img)
        {
            this.Img = img;
            СamStopped = false;
        }
        public ImageChangedEventArgs(Image img,string ImgPath)
        {
            this.Img = img;
            СamStopped = false;
            this.ImgPath = ImgPath;
        }
       // public bool UsePython{get; private set;}        
        public ImageChangedEventArgs(Image img, bool camStopped) : this(img)
        {            
            this.СamStopped = camStopped;
        }
        public Image Img { get; private set; }
        public string ImgPath { get; set; }
        public bool IsScanLayoutSet { get; set; }

    }
    
}
