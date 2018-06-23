﻿//#define withOutErrorParse
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
namespace PostUserActivity.Python
{
    public enum RETURN_PARAMS
    {
        boundingBox = 0,
        isGood = 1,
        problemCodeList = 2,
        cropImageShape = 3,
        imRGB = 4
    }

    /// <summary>
    /// предположим, что тут будет анализатор картинок
    /// </summary>


    public class PythonImageAnalyzer
    {
        private string pythonDetectorPath;

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static object _pythonObject = null;
        static Type _pythonServer = null;
        static string _pythonSrcpath = null;

        object[] initParams = null;

        private CommonLib.ConfigurationSettings config;

        public PythonImageAnalyzer(string pythonDetectorPath, CommonLib.ConfigurationSettings config)
        {
            this.config = config;
            initParams = new object[15];
            var uri = new System.Uri(AppDomain.CurrentDomain.BaseDirectory);
            var QALogDir = uri.AbsolutePath;
            var armSettings = config.GetArmSettings();

            //frame_rate=24, 
            //video_record_max_duration =100,   pConfig.VideoRecordMaxDuration
            //yaw_thres=0.2,                    pConfig.YawThreshold
            //roll_thres =0.2,                  pConfig.RollThreshold
            //pitch_thres =0.2,                 pConfig.PitchThreshold
            //blur_detection_thres=3.0,         pConfig.BlurDetectionThres
            //brightness_thres_low =0.3,        pConfig.BrithnessThresLow
            //brightness_thres_high =0.75,      pConfig.BrithnessThresHigh
            //lips_thres=0.04,                  pConfig.LipsThreshhold
            //lips_blur_thres =3.1c             pConfig.BlurThreshhold
            //yaw_mean=0.161,                   pConfig.YawMean
            //roll_mean=0.358,                  pConfif.RollMean
            //pitch_mean=-0.92                  pConfig.PitchMean
           
            this.pythonDetectorPath = pythonDetectorPath;
            initParams[0] = QALogDir;
            initParams[1] = pythonDetectorPath;
            initParams[2] = 24.ToString(CultureInfo.InvariantCulture);
            initParams[3] = armSettings.VideoRecordMaxDuration.ToString(CultureInfo.InvariantCulture);
            initParams[4] = armSettings.YawThreshold.ToString(CultureInfo.InvariantCulture);
            initParams[5] = armSettings.RollThreshold.ToString(CultureInfo.InvariantCulture);
            initParams[6] = armSettings.PitchThreshold.ToString(CultureInfo.InvariantCulture);
            initParams[7] = armSettings.BlurDetectionThres.ToString(CultureInfo.InvariantCulture);
            initParams[8] = armSettings.BrithnessThresLow.ToString(CultureInfo.InvariantCulture);
            initParams[9] = armSettings.BrithnessThresHigh.ToString(CultureInfo.InvariantCulture);
            initParams[10] = armSettings.LipsThreshhold.ToString(CultureInfo.InvariantCulture);
            initParams[11] = armSettings.BlurThreshhold.ToString(CultureInfo.InvariantCulture);
            initParams[12] = armSettings.YawMean.ToString(CultureInfo.InvariantCulture);
            initParams[13] = armSettings.RollMean.ToString(CultureInfo.InvariantCulture);
            initParams[14] = armSettings.PitchMean.ToString(CultureInfo.InvariantCulture);

        }

        
        void makeInit()
        {
            if (isInitDone == false)
            {   
                this.getPythonServerInstance().InvokeMember("postInit",
                   BindingFlags.InvokeMethod, null, this.getPythonObjectInstance(), initParams);
            }
            isInitDone = true;
        }

        bool isInitDone = false;

        public object getPythonObjectInstance()
        {
            try
            {
                if (_pythonObject == null || _pythonServer == null)
                {
                    _pythonServer = Type.GetTypeFromProgID("Python.QualityAnalyserCom");
                    _pythonObject = Activator.CreateInstance(_pythonServer);
                    isInitDone = false;
                    makeInit();
                }
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException == null ? "" : (ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                string ss = string.Format("Error: {0}, {1}Inner:{2}", ex.Message + Environment.NewLine + ex.StackTrace, Environment.NewLine, innerMsg);
                System.Diagnostics.Trace.WriteLine(ss);
                Console.WriteLine(ss);
                logger.Error("Text Mesage Exception: " + Environment.NewLine + ss);
                logger.Error(ex);
            }

            return _pythonObject;
        }
        string getPythonSrcPath()
        {
            if (_pythonSrcpath == null)
            {
                DirectoryInfo DirInf = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                try
                {
                    bool CheckDir = Directory.Exists(DirInf.Parent.FullName + "\\PythonSrc");
                    while (!CheckDir)
                    {
                        CheckDir = Directory.Exists(DirInf.Parent.FullName + "\\PythonSrc");
                        DirInf = DirInf.Parent;
                    }
                    string PathFolder = DirInf.FullName + "\\PythonSrc\\";
                    return PathFolder;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("CANT FIND PythonSrc");
                    logger.Error("CANT FIND PythonSrc");
                    //logger.ErrorException("Cant find PythonSrc", ex);
                    //throw ex;
                    return "PATH NOT FOUND";
                }
                return "Path NOT FOUND";
            }
            else return _pythonSrcpath;
        }


        public Type getPythonServerInstance()
        {
            try
            {
                if (_pythonObject == null || _pythonServer == null)
                {
                    _pythonServer = Type.GetTypeFromProgID("Python.QualityAnalyserCom");
                    _pythonObject = Activator.CreateInstance(_pythonServer);
                    isInitDone = false;
                    makeInit();
                }
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException == null ? "" : (ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                string ss = string.Format("Error: {0}, Inner:{1}", ex.Message + Environment.NewLine + ex.StackTrace, innerMsg);
                System.Diagnostics.Trace.WriteLine(ss);
                Console.WriteLine(ss);
                logger.Error(ex);
            }

            return _pythonServer;
        }
        /// <summary>
        /// return bitmap instead of rgbArray
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rgbArray"></param>
        /// <returns></returns>
        ///  
        public object InvokePythonMethod(string NameOfMember, object[] Params)
        {
            return this.getPythonServerInstance().InvokeMember(NameOfMember,
                    BindingFlags.InvokeMethod, null, this.getPythonObjectInstance(), Params);
        }
        public Bitmap BitmapFromPython(string WhichOne)
        {
            switch (WhichOne)
            {
                case "getCropim":
                    {
                        var cropImage = InvokePythonMethod("getCropim", new object[] { });
                        //get cropImage properties
                        var CropShapeArr = (object[])InvokePythonMethod("getRetValue", new object[] { RETURN_PARAMS.cropImageShape });
                        Bitmap tmp = new Bitmap((int)CropShapeArr[1], (int)CropShapeArr[0]);
                        return getBitMapFromPython(tmp, cropImage);
                    }
                case "getRgbim":
                    {
                        var RGBImage = InvokePythonMethod("getRgbim", new object[] { });
                        //get cropImage properties
                        var RGBShapeArr = (object[])InvokePythonMethod("getRetValue", new object[] { RETURN_PARAMS.imRGB });
                        Bitmap tmp = new Bitmap((int)RGBShapeArr[1], (int)RGBShapeArr[0]);
                        return getBitMapFromPython(tmp, RGBImage);
                    }
            }

            return null;

        }
        public Bitmap getBitMapFromPython(Bitmap image, object rgbArray)
        {
            var cropImageArray = (object[])rgbArray;


            int startX = 0;
            int startY = 0;
            int w = image.Width;
            int h = image.Height;
            int offset = 0;
            int scansize = image.Width;

            const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            if (image == null) throw new ArgumentNullException("image");
            if (rgbArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat);
            try
            {
                byte[] pixelData = new Byte[data.Stride];
                for (UInt32 scanline = 0; scanline < data.Height; scanline++)
                {
                    UInt32 uu = Convert.ToUInt32(data.Stride);
                    IntPtr ptr = data.Scan0;
                    IntPtr ptr2 = (IntPtr)(ptr.ToInt64() + (scanline * uu));

                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        pixelData[pixeloffset * PixelWidth + 0] = System.Convert.ToByte(cropImageArray[scanline * w * 3 + pixeloffset * 3 + 0]);
                        pixelData[pixeloffset * PixelWidth + 1] = System.Convert.ToByte(cropImageArray[scanline * w * 3 + pixeloffset * 3 + 1]);
                        pixelData[pixeloffset * PixelWidth + 2] = System.Convert.ToByte(cropImageArray[scanline * w * 3 + pixeloffset * 3 + 2]);
                    }

                    Marshal.Copy(pixelData, 0, ptr2, data.Stride);
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
            return image;
        }

        /// <summary>
        /// предположим, что это преобразование Bitmap -> int []
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rgbArray"></param>
        public void getRGB3D(Bitmap image, int[][][] rgbArray)
        {
            int startX = 0;
            int startY = 0;
            int w = image.Width;
            int h = image.Height;
            int offset = 0;
            int scansize = image.Width;

            const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            // En garde!
            if (image == null) throw new ArgumentNullException("image");
            if (rgbArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            //if (h < 0 || (rgbArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            try
            {
                byte[] pixelData = new Byte[data.Stride];
                for (UInt32 scanline = 0; scanline < data.Height; scanline++)
                {
                    UInt32 uu = Convert.ToUInt32(data.Stride);
                    IntPtr ptr = data.Scan0;
                    IntPtr ptr2 = (IntPtr)(ptr.ToInt64() + (scanline * uu));
                    Marshal.Copy(ptr2, pixelData, 0, data.Stride);
                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        rgbArray[scanline][pixeloffset][0] = pixelData[pixeloffset * PixelWidth + 0];
                        rgbArray[scanline][pixeloffset][1] = pixelData[pixeloffset * PixelWidth + 1];
                        rgbArray[scanline][pixeloffset][2] = pixelData[pixeloffset * PixelWidth + 2];
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
        }
        private static byte[] GetColorData(Bitmap bmp, bool reverseRGB)
        {
            var date1 = DateTime.Now;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            IntPtr ptr = bmpdata.Scan0;
            int bytes = bmpdata.Stride * bmp.Height;
            byte[] results = new byte[bytes];
            Marshal.Copy(ptr, results, 0, bytes);

            if (reverseRGB)
            {
                byte tmp;
                int pos = 0;
                while (pos + 2 < bytes)
                {
                    tmp = results[pos];
                    results[pos] = results[pos + 2];
                    results[pos + 2] = tmp;
                    pos += 3;
                }
            }

            bmp.UnlockBits(bmpdata);
            var date2 = DateTime.Now;
            Console.WriteLine("GetColorDate time :" + (date2 - date1).ToString());
            return results;
        }
#if(withOutErrorParse)
        /// <summary>
        /// анализ изображений
        /// </summary>
        /// <param name="img"></param>
        public ImageProcessResult Analyze(Image item,int deviceType)
        {
#else
        public ImageProcessResult Analyze(Image item)
        {
#endif            
            try
            {
                //create python COM instance, which will invoke python methods
                //for tests
                //item.Save("C:\\init\\SharpImageBeforeConvertationToArray.jpg");
                object pythonObject = this.getPythonObjectInstance();
                Type pythonServer = this.getPythonServerInstance();
                logger.Info("Send Python Face Detector path: {0}", pythonDetectorPath);
                
                Console.WriteLine(pythonObject.ToString());
                var frame =   new Bitmap(item);
                var RGBARRay = GetColorData(new Bitmap(item), false);
                // lets get it started. in here :D
                //bitmap to []int  in order to pass rgb array to python     
              /*
                var date1 = DateTime.Now;
                var frame = new Bitmap(item);
                var bitmapInRgbFormat = frame.Clone(
                   new Rectangle(0, 0, frame.Width, frame.Height),
                   PixelFormat.Format32bppRgb);
                int[][][] rgbArray = new int[frame.Height][][];
                for (int i = 0; i < frame.Height; i++)
                {
                    rgbArray[i] = new int[frame.Width][];
                    for (int j = 0; j < frame.Width; j++)
                        rgbArray[i][j] = new int[3];
                }

                getRGB3D(frame, rgbArray);
                Int32[] finalArray = new Int32[frame.Height * frame.Width * 3];

                for (int i = 0; i < frame.Height; i++)
                {
                    for (int j = 0; j < frame.Width; j++)
                    {
                        for (int k = 0; k < 3; k++)
                            finalArray[i * frame.Width * 3 + j * 3 + k] = (Int32)rgbArray[i][j][k];
                    }
                }
                var date2 = DateTime.Now;
                Console.WriteLine("Our Image to Array time : " + (date2 - date1).ToString());
               */

                //var uri = new System.Uri(getPythonSrcPath()); //TODO set this directory for installer
                //path where EXE starts
                //  var test = AppDomain.CurrentDomain.BaseDirectory;
                // Debug.WriteLine(test);
               
               
 

                //OUR SENDING
               // object result = InvokePythonMethod("check_frame", new object[] { finalArray, frame.Height, frame.Width });
                
                //Mine SENDING
                object result = InvokePythonMethod("check_frame", new object[] { RGBARRay, frame.Height, frame.Width });
                
                
               
                
                
                
                bool ImgIsgood = (bool)InvokePythonMethod("getRetValue", new object[] { RETURN_PARAMS.isGood });

                object[] RetValueList = (object[])InvokePythonMethod("getRetValue", new object[] { RETURN_PARAMS.problemCodeList });

                int Count = RetValueList.Count();

                int[] problem_list = new int[Count];

                for (int i = 0; i < Count; i++)
                {
                    problem_list[i] = (int)RetValueList[i];
                }

                Bitmap fullImage = null;
                Bitmap cropImage = null;

                //fullImage = BitmapFromPython("getRgbim");
                if (problem_list.Count() == 0 && ImgIsgood)
                {
                    fullImage = BitmapFromPython("getRgbim");
                    cropImage = BitmapFromPython("getCropim");
                }
                #if(withOutErrorParse)
                if (deviceType == 2) // для сканера всегда у нас все хорошо!
                {
                    fullImage = BitmapFromPython("getRgbim");
                    cropImage = BitmapFromPython("getCropim");
                    problem_list = new int[0];
                }
                #endif

                return new ImageProcessResult(ImgIsgood, cropImage, fullImage, problem_list);
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException == null ? "" : (ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                string ss = string.Format("Error: {0}, Inner:{1}", ex.Message + Environment.NewLine + ex.StackTrace, innerMsg);
                System.Diagnostics.Trace.WriteLine(ss);
                Console.WriteLine(ss);
                logger.Error(ex);
            }
            return new ImageProcessResult(false, null, null, new int[1] { 1 });
        }

        public Bitmap getFullImage()
        {
            return BitmapFromPython("getRgbim");
        }
    }
}