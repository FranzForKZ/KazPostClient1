using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Data;

using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;


namespace PostUserActivity.Python
{
    public class FaceDetectorProcessor
    {
        Image<Bgr, byte> _input;
        static CascadeClassifier Face = null;

        static object lockObject = new object();

        private NLog.Logger logger;
        public FaceDetectorProcessor(Image<Bgr, byte> input)
        {
            Image<Bgr, byte> input2 = input;

            //_input = input2.Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            _input = input2;
            if (Face == null)
            {
                Face = new CascadeClassifier("./Cascades/haarcascade_frontalface_default.xml");//Our face detection method 
            }
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public Rectangle[] isFaceDetected()
        {
            System.Diagnostics.Debug.WriteLine("isFaceDetected started");
            Image<Gray, byte> gray_frame = null;
            gray_frame = _input.Convert<Gray, Byte>();
            Rectangle[] facesDetected = new Rectangle[0];
            try
            {
                //Rectangle[] facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);
                facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
                
            System.Diagnostics.Debug.WriteLine("isFaceDetected stopped");
            if (facesDetected != null && facesDetected.Count() == 1)
                return facesDetected;
            return facesDetected;
        }

        /// <summary>
        /// расширим квадрат, для того, что бы не обрезалось изображение, scale - доля, на которую увеличиваем
        /// </summary>
        public static Func<Rectangle[], double, Rectangle[]> squareExtension = (rects, scale) =>
       {
           Rectangle[] result = new Rectangle[rects.Length];
           for (var i = 0; i < rects.Length; i++)
           {
               //var wScale = Convert.ToInt32(rects[i].Width * scale);
               //var hScale = Convert.ToInt32(rects[i].Height * scale);
               var wScale = 10;
               var hScale = 10;

               int y = rects[i].Top;
               if (rects[i].Top - hScale > 0)
               {
                   y = rects[i].Top - (int)hScale;
               }
               int x = rects[i].Left;
               if (rects[i].Left - wScale > 0)
               {
                   x = rects[i].Left - (int)wScale;
               }
               int width = rects[i].Width + wScale * 2;
               int heigth = rects[i].Height + hScale * 2;
               var rect = new Rectangle(x, y, width, heigth);
               result[i] = rect;
           }

           return result;
       };
    }
}
