using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts.HWContracts;
using Tevian;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.IO;

namespace PostUserActivity.Tevian
{
    public class AnalyseResult
    {
        public Bitmap Full { get; set; }
        public Bitmap Cropped { get; set; }
        public AnalyzeImageResultType Errors { get; set; }

        
        
    }
    public class BackGrndArgs
    {
        public ImageChangedEventArgs ImgArgs {get;set;}
        public DeviceType Device { get; set; }
    }
    public class Analyser : PostUserActivity.Contracts.IImageAnalyzer
    {
        System.ComponentModel.BackgroundWorker backgoundThread_ImageAnalizer;
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        ImageAnalyze Tevian_Analyser;
        public Rectangle FaceCoordinates { get; private set; }
        private bool CheckImageDemenions(ImageDimension[] ID)
        {
            if(ID != null)
            {
                string ForLogger = "";
                for (int i = 0; i < ID.Length; i++)
                {
                    ForLogger += ID[i].DimensionName + ": " + ID[i].DimensionValue + "\n";
                    if (ID[i].DimensionName == "Лицо сильно повернуто по одной из осей")
                    {
                        logger.Info(ForLogger);
                        return false;
                    }
                }
                logger.Info(ForLogger);
                return true;
            }
            return false;
            
        }
        private List<Contracts.AnalyzeImageResultType> FlagsToList(AnalyzeImageResultType Enum)
        {
            var List = new List<Contracts.AnalyzeImageResultType>();
            if ((Enum & AnalyzeImageResultType.NoError) == AnalyzeImageResultType.NoError)
            {
                List.Add(Contracts.AnalyzeImageResultType.NoError);
            }
            if ((Enum & AnalyzeImageResultType.ClosedEye) == AnalyzeImageResultType.ClosedEye)
            {
                List.Add(Contracts.AnalyzeImageResultType.СlosedEye);
            }
            if ((Enum & AnalyzeImageResultType.FaceBlurred) == AnalyzeImageResultType.FaceBlurred)
            {
                List.Add(Contracts.AnalyzeImageResultType.FaceBlurred);
            }
            if ((Enum & AnalyzeImageResultType.FaceMarred) == AnalyzeImageResultType.FaceMarred)
            {
                List.Add(Contracts.AnalyzeImageResultType.FaceMarred);
            }
            if ((Enum & AnalyzeImageResultType.FaceNotFound) == AnalyzeImageResultType.FaceNotFound)
            {
                List.Add(Contracts.AnalyzeImageResultType.FaceNotFound);
            }
            if ((Enum & AnalyzeImageResultType.FaceRotated) == AnalyzeImageResultType.FaceRotated)
            {
                List.Add(Contracts.AnalyzeImageResultType.FaceRotated);
            }
            if ((Enum & AnalyzeImageResultType.InnerException) == AnalyzeImageResultType.InnerException)
            {
                List.Add(Contracts.AnalyzeImageResultType.InnerException);
            }
            if ((Enum & AnalyzeImageResultType.MoreThanOnePerson) == AnalyzeImageResultType.MoreThanOnePerson)
            {
                List.Add(Contracts.AnalyzeImageResultType.MoreThanOnePerson);
            }
            if ((Enum & AnalyzeImageResultType.NotPerson) == AnalyzeImageResultType.NotPerson)
            {
                List.Add(Contracts.AnalyzeImageResultType.PhotoFace);
            }
            return List;
        }
            //   HashSet<PostUserActivity.Contracts.AnalyzeImageResultType> webCamErrors;
        private AnalyseResult WebCamAnalyse(ImageChangedEventArgs imgArgs)
        {
            var obj = new AnalyseResult();
                var tempBitmap = (Bitmap)imgArgs.Img;
                
                var tempResult = Tevian_Analyser.AnalyseCamImage(tempBitmap);
                
                obj.Full = tempBitmap;
                obj.Errors = tempResult.Errors;
                //костыль чтобы не падала
                if (tempResult.Errors == AnalyzeImageResultType.InnerException)
                    return obj;
                //рамко
                if (tempBitmap != null)
                {
                    try
                    {
                        //рисовать мы будем в другом месте...
                        //using (Graphics G = Graphics.FromImage(tempBitmap))
                        //{
                        if (tempResult.Faces.Length != 0)
                        {
                            FaceCoordinates = new Rectangle(tempResult.Faces[0].X, tempResult.Faces[0].Y, tempResult.Faces[0].Width, tempResult.Faces[0].Height);
                            //положить в кроп найденное лицо 
                            //if (CheckImageDemenions(tempResult.ImageDimensions))
                            //{
                            //"Лицо сильно повернуто по одной из осей" отсутствует
                            obj.Cropped = CommonLib.ImageExtensions.CropImage(new Bitmap(tempBitmap), FaceCoordinates);
                            //}
                            //  G.DrawRectangle(new Pen(Color.Yellow), Rect);

                            return obj;
                        }
                        else
                        {
                            //пустой ректангл, если не нашли лица(для логики отрисовки)
                            FaceCoordinates = new Rectangle();
                            return obj;
                        }
                    }
                    //}
                    catch (Exception ex)
                    {

                        logger.Error(ex.Message);
                        logger.Error(ex.InnerException);
                    }

                }
            return obj;
            //return null;
        }
        private AnalyseResult ScannerAnalyse(ImageChangedEventArgs imgArgs)
        {
            var tempBitmap = (Bitmap)imgArgs.Img;
            return RotateOnAngle(tempBitmap, Tevian_Analyser);
        }
        private AnalyseResult RotateOnAngle(Bitmap tempBitmap, ImageAnalyze Tevian_Analyser)
        {
            AnalyzeImageResultType Error = AnalyzeImageResultType.FaceNotFound;
            var tempResult = Tevian_Analyser.AnalyseScanImage(tempBitmap);
            Error = tempResult.Errors;
            if(Error!= AnalyzeImageResultType.InnerException)
            {
                    if (tempResult.Faces.Length != 0) //&& (tempResult.Document.Height < tempResult.Document.Width) пока не ищет координаты документа...закоментил
                    {
                        double angle=0;
                        foreach (var item in tempResult.ImageDimensions)
                        {
                            if(item.DimensionName == "Угол")
                            {
                                angle = double.Parse(item.DimensionValue);
                            }
                        }
                     tempBitmap = RotateImage(tempBitmap, angle);
                     tempBitmap.Save("MediaFolder\\AfterRotate_" + DateTime.Now.ToString("MMss") + ".jpg");
                        tempResult = Tevian_Analyser.AnalyseScanImage(tempBitmap);
                        foreach (var item in tempResult.ImageDimensions)
                        {
                            logger.Info("{0},{1}", item.DimensionName, item.DimensionValue);
                        }
                        if (tempResult.Faces.Length != 0) //&& (tempResult.Document.Height < tempResult.Document.Width) пока не ищет координаты документа...закоментил
                        {
                            FaceCoordinates = new Rectangle(tempResult.Faces[0].X, tempResult.Faces[0].Y, tempResult.Faces[0].Width, tempResult.Faces[0].Height);
                            return new AnalyseResult { Full = tempBitmap, Cropped = CommonLib.ImageExtensions.CropImage(new Bitmap(tempBitmap), FaceCoordinates), Errors = tempResult.Errors };
                        }
                        else
                        {
                           // Rectangle Rect = new Rectangle(tempResult.Faces[0].X, tempResult.Faces[0].Y, tempResult.Faces[0].Width, tempResult.Faces[0].Height);
                            return new AnalyseResult { Full = tempBitmap, Cropped = null, Errors = tempResult.Errors };
                        }
                        
                    }
            }
            return new AnalyseResult { Full = tempBitmap, Cropped = null, Errors = Error };  
        }
        private Bitmap RotateImage(Bitmap img, double rotationAngle)
        {
            logger.Info("Поворачиваем исходное изображение на " + rotationAngle + "градусов");
           // rotationAngle += 10;
            Bitmap rotatedImage = (Bitmap)img.Clone();
            //Bitmap rotatedImage = new Bitmap(img.Width, img.Height);
            //create an empty Bitmap image
            using (Graphics gfx = Graphics.FromImage(rotatedImage))
            {
                 //turn the Bitmap into a Graphics object
            gfx.Clear(Color.White);
               
            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.TranslateTransform( img.Width / 2, img.Height / 2 );
            //now rotate the image
            gfx.RotateTransform((float)rotationAngle);

            gfx.TranslateTransform( -img.Width / 2, -img.Height / 2 );            

            //now draw our new image onto the graphics object
            gfx.DrawImage(img, new Point(0, 0));
            }

            rotatedImage.Save(Path.Combine("MediaFolder",DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_fffff") + ".TIFF"), System.Drawing.Imaging.ImageFormat.Tiff);
            //return the image
            return rotatedImage;
        }
       public void StartCamAnalyze()
        {
            Tevian_Analyser.StartCamAnalyse();
        }
       public void StopCamAnalyze()
       {
           Tevian_Analyser.StopCamAnalyse();
       }
        public void Analyze(ImageChangedEventArgs imgArgs, DeviceType device)
        {
            if (!backgoundThread_ImageAnalizer.IsBusy)
            {
                backgoundThread_ImageAnalizer.RunWorkerAsync(new BackGrndArgs() { ImgArgs = imgArgs, Device = device });
            }
            else
            {
                if (device == DeviceType.WebCam && imgArgs.СamStopped)
                {
                    AnalyzeCompleted(this, new PostUserActivity.Contracts.AnalyzeCompletedEventArgs(device, new List<Contracts.AnalyzeImageResultType>() { Contracts.AnalyzeImageResultType.FaceNotFound }));
                }
            }
            
            //if (imgArgs.Img !=null)
            //{
            //    imgArgs.Img.Dispose();    
            //}
            
        }
        private void saveImage(ImageChangedEventArgs imgArgs)
        {
           string PathForSave =  System.Configuration.ConfigurationManager.AppSettings["FolderForSavingImages"];
            if(PathForSave != string.Empty)
            {
                PathForSave = System.IO.Path.Combine(PathForSave, DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_fffff") + ".jpg");
                 imgArgs.Img.Save(PathForSave, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
        public event EventHandler<PostUserActivity.Contracts.AnalyzeCompletedEventArgs> AnalyzeCompleted = (sender, e) => { };
        public event EventHandler<PostUserActivity.Contracts.AnalyzeCompletedEventArgs> AnalyzeImage = (sender, e) => { };
        public Analyser()
        {
            Tevian_Analyser = new ImageAnalyze();
            Tevian_Analyser.Init(3.0m, 3.1m, 0.75m, 0.3m, 0.04m, 0.025m, 0.358m, 0.025m, -0.2m, 0.358m, 0.2m);
            backgoundThread_ImageAnalizer = new BackgroundWorker();
            backgoundThread_ImageAnalizer.WorkerSupportsCancellation = true;
            backgoundThread_ImageAnalizer.DoWork += bwImageAnalizer_DoWork;
         //   webCamErrors = new HashSet<PostUserActivity.Contracts.AnalyzeImageResultType>();
        }
        ~Analyser()
        {
            if (backgoundThread_ImageAnalizer != null)
            {
                if (backgoundThread_ImageAnalizer.IsBusy)
                {
                    backgoundThread_ImageAnalizer.CancelAsync();    
                }
                backgoundThread_ImageAnalizer.Dispose();
            }
            if (Tevian_Analyser != null)
            {
                Tevian_Analyser.Dispose();
            }
        }
        private void bwImageAnalizer_DoWork(object sender, DoWorkEventArgs e)
        {
            
           var Args = (BackGrndArgs)e.Argument;
           var imgArgs = Args.ImgArgs;
           var device = Args.Device;
           logger.Debug("Пришел кадр на анализ с устройства " + device.ToString());
           
            saveImage(imgArgs);
            if (!string.IsNullOrEmpty(imgArgs.ImgPath))
                logger.Info("file path: " + imgArgs.ImgPath);
            AnalyseResult Result = null;
            switch (device)
            {
                case DeviceType.WebCam:
                    {
                        Result = WebCamAnalyse(imgArgs);
                        break;
                    }

                case DeviceType.Scanner:
                    {

                        Result = ScannerAnalyse(imgArgs);
                        break;
                    }
                case DeviceType.HDDrive:
                    {
                        Result = WebCamAnalyse(imgArgs);
                        break;
                    }
                default:
                    {

                        break;
                    }
                    
            }
            //после анализа изображение нам уже не нужно в памяти...
            logger.Debug("Получили результат:");
            List<Contracts.AnalyzeImageResultType> ResultList = FlagsToList(Result.Errors);
            foreach (var item in ResultList)
            {
                logger.Debug(item);
            }
            //Args.ImgArgs.Img.Dispose();
            if (Result.Errors != AnalyzeImageResultType.NoError)
            {
                if (device == DeviceType.Scanner)
                {
                    logger.Info("**********************Scan completed***********************");
                    AnalyzeImage(this, new PostUserActivity.Contracts.AnalyzeCompletedEventArgs(device, ResultList));
                    AnalyzeCompleted(this, new PostUserActivity.Contracts.AnalyzeCompletedEventArgs(device, ResultList));
                    return;
                }
                else
                {
                    if (!imgArgs.СamStopped)
                    {
                        AnalyzeImage(this, new PostUserActivity.Contracts.AnalyzeCompletedEventArgs(device, ResultList));
                        return;
                    }
                    else
                    {
                        logger.Info("**********************Cam Stoped***********************");
                        AnalyzeCompleted(this, new PostUserActivity.Contracts.AnalyzeCompletedEventArgs(device, ResultList));
                        return;
                    }
                }
            }
            else
            {
                AnalyzeCompleted(this, new PostUserActivity.Contracts.AnalyzeCompletedEventArgs(device, ResultList, (Image)Result.Full, (Image)Result.Cropped));
                logger.Info("**********************Face found***********************");
                return;

            }
        }
    }
}
