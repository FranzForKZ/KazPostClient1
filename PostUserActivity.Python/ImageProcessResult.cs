using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Python
{
    public class ImageProcessResult
    {
        public Boolean isGood;
        public Bitmap CroppedImage;
        public Bitmap RGBImage;
        public int[] ProblemList;
        public bool isCamStopped;

        //public ImageProcessResult(Boolean isGood,Bitmap croppedImage,  Bitmap RGBImage)
        // {
        //     this.isGood = isGood;
        //     this.croppedImage = croppedImage;
        //     this.RGBImage = RGBImage;
        // }

        public ImageProcessResult(bool isGood, Bitmap croppedImage, Bitmap RGBImage, int[] ProblemList)
        {
            this.isGood = isGood;
            this.CroppedImage = croppedImage;
            this.RGBImage = RGBImage;
            this.ProblemList = ProblemList;
            this.isCamStopped = false;
        }

        public ImageProcessResult(bool isGood, Bitmap croppedImage, Bitmap RGBImage, int[] ProblemList, bool isCamLastFrame)
        {
            this.isGood = isGood;
            this.CroppedImage = croppedImage;
            this.RGBImage = RGBImage;
            this.ProblemList = ProblemList;
            this.isCamStopped = isCamLastFrame;
        }

        public DeviceType Device { get; set; }
        public Image BaseImage { get; set; }
    }
}
