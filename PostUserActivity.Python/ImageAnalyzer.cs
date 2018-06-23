//#define withOutErrorParse
//#define RussianPasport
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.HWContracts;
using Emgu.CV;
using Emgu.CV.Structure;
using System.ComponentModel;
using System.Configuration;
using NLog;
using ScanProcessor;
using System.Threading;
using ConfigurationSettings = CommonLib.ConfigurationSettings;
using System.Globalization;
using System.IO;

//using System.Collections.

namespace PostUserActivity.Python
{

    /// <summary>
    /// предположим, что тут будет анализатор картинок
    /// </summary>
    public class ImageAnalyzer : IImageAnalyzer
    {
        Image<Bgr, Byte> currentFrame; //current image aquired from webcam for display
        Image<Gray, byte> result, TrainedFace = null; //used to store the result image and trained face
        Image<Gray, byte> gray_frame = null; //grayscale current image aquired from webcam for processing

        private Logger logger;
        private ConfigurationSettings config;
        HashSet<AnalyzeImageResultType> webCamErrors = new HashSet<AnalyzeImageResultType>();

        //int _counter = 0;
        //bool _isStopped = false;
        System.ComponentModel.BackgroundWorker backgoundThread_ImageAnalizer;

        bool isFinalGoodImageFound = false;

        private string pythonDetectorPath;

        #region ctor
        public ImageAnalyzer(string pythonDetectorPath, CommonLib.ConfigurationSettings config)
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
            //logger.Info("Entering ImageAnalyzer constructor");
            this.config = config;

            backgoundThread_ImageAnalizer = new BackgroundWorker();
            backgoundThread_ImageAnalizer.WorkerSupportsCancellation = true;
            backgoundThread_ImageAnalizer.DoWork += bwImageAnalizer_DoWork;
            backgoundThread_ImageAnalizer.RunWorkerCompleted += bwImageAnalizer_RunWorkerCompleted;
            backgoundThread_ImageAnalizer.ProgressChanged += bwImageAnalizer_ProgressChanged;
            this.pythonDetectorPath = pythonDetectorPath;

            //logger.Info("background workers added!");
        }

        public ImageAnalyzer(string pythonDetectorPath, CommonLib.ConfigurationSettings config, IHWDeviceWork webCam)
            : this(pythonDetectorPath, config)
        {
            this.WebCam = webCam;
        }

        private IHWDeviceWork WebCam { get; set; }

        #endregion

        void bwImageAnalizer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private class ImgAnalyzerDto
        {
            public Image<Bgr, Byte> Img { get; set; }
            public DeviceType Device { get; set; }
            public bool isCamStopped { get; set; }
            public Image BaseImage { get; set; }

            public string ImgFilePath { get; set; }
        }
        private class FindResult
        {
            public List<FindResult>  RList {get;private set;}
            public   bool isFaceFound { get; set; }
            public Image faceImage { get; set; }
            public FindResult(bool faceFound,Image face)
            {
                isFaceFound = faceFound;
                faceImage = face;
                RList = new List<FindResult>();
            }
            public void update(bool faceFound,Image face)
            {
                RList.Add(new FindResult(faceFound,face));
                if(faceFound)
                {
                    this.isFaceFound = true;
                    this.faceImage = face;
                }
            }
            public int GoodFacesFound()
            {
                int count = 0;
                foreach (var item in RList)
                {
                    if(item.isFaceFound)
                    {
                        count++;
                    }
                }
                return count;
            }

        }
        private FindResult FindFace(Image InputImage)
        {
            int counter = 0;
            float face_width, face_height, face_delta_min, face_delta_max;
            int dpi;
            try
            {

                face_width = float.Parse(ConfigurationManager.AppSettings["face_width"], System.Globalization.CultureInfo.InvariantCulture);
                face_height = float.Parse(ConfigurationManager.AppSettings["face_height"], System.Globalization.CultureInfo.InvariantCulture);
                face_delta_min = float.Parse(ConfigurationManager.AppSettings["face_delta_min"], System.Globalization.CultureInfo.InvariantCulture);
                face_delta_max = float.Parse(ConfigurationManager.AppSettings["face_delta_max"], System.Globalization.CultureInfo.InvariantCulture);
                dpi = int.Parse(ConfigurationManager.AppSettings["ScannerDpi"]);


            }
            catch
            {
                //Console.WriteLine("Не смог считать параметры face_wigth и.т.д.");
                logger.Error("Не смог считать параметры face_wigth и.т.д.");
                face_width = 2.0f;
                face_height = 2.6f;
                face_delta_min = 0.2f;
                face_delta_max = 0.9f;
                dpi = 300;
            }
            double minRectangleSquare = face_width * face_height * face_delta_min;
            double maxRectamgleSquare = face_width * face_height * face_delta_max;

            int goodFacesFound = 0;
            var Result = new FindResult(false,null);
            while (counter <= 3 )
            {
                FaceDetectorProcessor scanFaceDetectorProcessor = new FaceDetectorProcessor(new Image<Bgr, byte>(new Bitmap(InputImage)));
                Rectangle[] facesOnScan = scanFaceDetectorProcessor.isFaceDetected();
                goodFacesFound = 0;
                double RectangleSquare = 0;
                foreach (var item in facesOnScan)
                {
                    RectangleSquare = ((double)item.Width / Math.Round(InputImage.HorizontalResolution) * 2.54 * ((double)item.Height / Math.Round(InputImage.HorizontalResolution)) * 2.54);
                    var croppedFace = DocDetector.CropImage(new Bitmap(InputImage), item); 
                    if (checkFaceSquare(RectangleSquare, minRectangleSquare, maxRectamgleSquare))
                     {
                         
                         croppedFace.Save("ScanCropedFaceGood" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");
                         Result.update(true, croppedFace);
                         goodFacesFound++;
                     }
                     else
                     {
                         Result.update(false, croppedFace);
                         croppedFace.Save("ScanCropedFaceBad" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");
                     }
                }
                if (goodFacesFound == 1)
                {
                    return Result;
                   //return new FindResult(true,)
                }
                InputImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                scanFaceDetectorProcessor = null;
                facesOnScan = null;
                counter++;
                System.Windows.Forms.Application.DoEvents();
            }
            return Result;
        }
        bool checkFaceSquare(double FoundFaceSquare, double minSquare,double MaxSquare)
        {


            if (FoundFaceSquare >= minSquare && FoundFaceSquare <= MaxSquare)
            {

                return true;

            }
            return false;
        }
        string path = ConfigurationManager.AppSettings["FolderForSavingImages"];
        void bwImageAnalizer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            //logger.Info("We returned from Python: bwImageAnalizer_RunWorkerCompleted");
            ImageProcessResult pythonResult = (ImageProcessResult)e.Result;
            List<AnalyzeImageResultType> errors = new List<AnalyzeImageResultType>();

            if (pythonResult.isGood)
            {
                if (pythonResult.Device == DeviceType.WebCam)
                {
                    isFinalGoodImageFound = true;
                }
                //Console.WriteLine("Python : Good Face FOUND!");
                logger.Info("Python : Good Face FOUND!");
            }
            else
            {
                //Console.WriteLine("Python was not able to detect good face. VERY BAD FACE DETECTED!");
                logger.Info("Python was not able to detect good face. VERY BAD FACE DETECTED!");
                
                if (pythonResult.ProblemList != null)
                {
                    errors = pythonResult.ProblemList.Select(p => (AnalyzeImageResultType)p).ToList();

                    var errorsDescr = string.Join(", ", AnalyzeImageResultDescriptions.ErrorsDescription(pythonResult.ProblemList.Select(p => (AnalyzeImageResultType)p).ToList()).ToArray());
                    var errorMsg = "Python Problems: " + errorsDescr + " (" + string.Join(", ", pythonResult.ProblemList.Select(p => p.ToString()).ToArray()) + ")";

                    logger.Info(errorMsg);
                    //Console.WriteLine(errorMsg);

                    if (pythonResult.Device == DeviceType.WebCam)
                    {
                        // add error to the list if error was not in the list before
                        for (int i = 0; i < pythonResult.ProblemList.Length; i++)
                        {
                            AnalyzeImageResultType eTyped = (AnalyzeImageResultType)pythonResult.ProblemList[i];
                            if (!webCamErrors.Contains(eTyped))
                                webCamErrors.Add(eTyped);
                        }
                    }
                }
            }


#if (withOutErrorParse)
            //билд по просьбе Романа, тут ошибки сканера просто пропускаются       
            if (pythonResult.Device == DeviceType.Scanner)
            {
                AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(pythonResult.Device, new List<AnalyzeImageResultType>(), pythonResult.BaseImage, pythonResult.CroppedImage));
                logger.Info("билд по просьбе Романа, тут ошибки сканера просто пропускаются. Выводим, что мы успешно нашли изображение");
                return;
            }
#endif




            if (pythonResult.isGood == true)
            {
                //Console.WriteLine("Python returned GOOD");
                logger.Info("Python returned GOOD");

                if (pythonResult.Device == DeviceType.WebCam)
                {

                    if (System.IO.Directory.Exists(path))
                    {
                        string ForSave =  Path.Combine(path , "Success_full_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_ff") + ".jpg");
                        logger.Info(string.Format("Сохраняем полный кадр {0} от питона", ForSave));
                        pythonResult.RGBImage.Save(ForSave);

                        ForSave = Path.Combine(path , "Success_preview_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_ff") + ".jpg");
                        logger.Info(string.Format("Сохраняем  найденное лицо {0} от питона", ForSave));
                        pythonResult.CroppedImage.Save(ForSave);
                    }
                    
                    AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(pythonResult.Device, errors, pythonResult.RGBImage, pythonResult.CroppedImage));
                }
                else
                {
                    AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(pythonResult.Device, errors, pythonResult.BaseImage, pythonResult.CroppedImage));
                }
                //logger.Info("Exiting bwImageAnalizer_RunWorkerCompleted 1");
                return;
            }


            if (pythonResult.Device == DeviceType.WebCam && backgoundThread_ImageAnalizer.IsBusy == false && pythonResult.isCamStopped)
            {
                AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(pythonResult.Device, webCamErrors.ToList(), pythonResult.RGBImage, pythonResult.CroppedImage));
                return;
            }
            else
            {
                if (WebCam != null)
                {
                    //logger.Info("WebCam is Recording: {0}", WebCam.IsRunning());
                }

             
                if (WebCam != null)
                {
                    //Console.WriteLine("WebCam is Recording: {0}", WebCam.IsRunning());
                }
                //  смысл такой: передаем в аналайзер текущее состояние камеры, тк возникает ситуация, (из-за асинхронности)
                //  когда мы мы проверяем кадр и в нем состояние камеры запущена, но в реальности это уже последний кадр и сама камера уже остановлена
                //  из-за устновленного флага pythonResult.isCamStopped == false, и того, что изображение не распознано, просиходит пропуск этого кадра и 
                //  на форме просто замирает последний кадр, без отображения результатов анализа (обычно это ошибка)
                //  TODO: Саше протестить на то, что происходит, если результат анализа положительный
                //  потом причесать код и провести рефракторинг
                if (pythonResult.Device == DeviceType.WebCam && backgoundThread_ImageAnalizer.IsBusy == false && WebCam != null && !WebCam.IsRunning())
                {
                    AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(pythonResult.Device, webCamErrors.ToList(), pythonResult.RGBImage, pythonResult.CroppedImage));
                }


            }

            // if we have scanner only
            if (pythonResult.Device == DeviceType.Scanner)
            {
                AnalyzeImage(this, new AnalyzeCompletedEventArgs(pythonResult.Device, errors, pythonResult.RGBImage, null));
                //logger.Info("Exiting bwImageAnalizer_RunWorkerCompleted 2");
                return;
            }
            logger.Info("Analyze Completed==============");
        }
        void bwImageAnalizer_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.Info("Analyze Started==============");
            logger.Info("Send img to Python");
            var dto = (ImgAnalyzerDto)e.Argument;
            logger.Info("File Path " + dto.ImgFilePath);
            if(File.Exists(dto.ImgFilePath))
            {
                string SaveFolder = ConfigurationManager.AppSettings["FolderForSavingImages"];
                if(SaveFolder != string.Empty)
                {
                    File.Copy(dto.ImgFilePath, Path.Combine(SaveFolder , Path.GetFileName(dto.ImgFilePath)));
                }   
            }
            else
            {
                string SaveFolder = ConfigurationManager.AppSettings["FolderForSavingImages"];
                if (!string.IsNullOrEmpty(SaveFolder))
                {
                    if (Directory.Exists(SaveFolder))
                    {
                        SaveFolder = Path.Combine(SaveFolder, DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_ffff") + ".jpg");
                        dto.Img.Save(SaveFolder);
                    }
                }
                
            }
            //Image<Bgr, Byte> originalFrame = (Image<Bgr, Byte>)e.Argument;
            Image<Bgr, Byte> originalFrame = dto.Img;

            PythonImageAnalyzer ia = new PythonImageAnalyzer(pythonDetectorPath, config);
            ImageProcessResult pythonResult;
#if (withOutErrorParse)
            pythonResult = ia.Analyze((Image)originalFrame.Bitmap, (int)dto.Device);
#else
            pythonResult = ia.Analyze((Image)originalFrame.Bitmap);
#endif
            pythonResult.Device = dto.Device;
            pythonResult.isCamStopped = dto.isCamStopped;
            pythonResult.BaseImage = dto.BaseImage;
            e.Result = pythonResult;
            logger.Info("Get img from Python");
        }
        private Image CheckFramePosition(Image imageToCheck)
        {
            try
            {
                Rectangle rec = DocDetector.FindSquare(new Image<Bgr, Byte>(new Bitmap((Image)imageToCheck))); // находим положение паспорта
                Bitmap croppedImage = DocDetector.CropImage(new Bitmap(imageToCheck), rec); // вырезаем паспорт
                if (croppedImage.Height > croppedImage.Width)
                {
                    imageToCheck.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    
                }
                return imageToCheck;
            }
            catch (Exception)
            {
                
                //Console.WriteLine("Coulnt find document");
                logger.Error("Coulnt find document");
                return imageToCheck;
            }
            
        }
        private void ProcessNewImageFromDevice(Image inputImage, DeviceType device, ImageChangedEventArgs imgArgs)
        {
            if (DeviceType.Scanner == device)
            {
                //ConfigurationManager.RefreshSection("appSettings");
                //var ScanJpg = (ConfigurationManager.AppSettings["ScanJpgPath"]);
                //if (ScanJpg != "")
                //{
                //    try
                //    {
                //        var temp = Image.FromFile(ScanJpg);
                //        inputImage = temp;
                //    }
                //    catch
                //    {
                //        logger.Info("ScanJpg FILE NOT FOUND");
                //    }
                //    logger.Info("ScanJpgPath in config not null!We CHANGE analyze image!!!");
                //}
            }
            var baseImg = (Image)inputImage.Clone();
            bool isFaceFound = false;
            Image<Bgr, Byte> originalFrame = new Image<Bgr, Byte>(new Bitmap((Image)inputImage.Clone())); // берем оргинальную картинку
            if (device == DeviceType.Scanner)
            {
                var errs = new List<AnalyzeImageResultType>();
                float face_width, face_height, face_delta_min, face_delta_max;
                int dpi;
                try
                {
                    face_width = float.Parse(ConfigurationManager.AppSettings["face_width"], System.Globalization.CultureInfo.InvariantCulture);
                    face_height = float.Parse(ConfigurationManager.AppSettings["face_height"], System.Globalization.CultureInfo.InvariantCulture);
                    face_delta_min = float.Parse(ConfigurationManager.AppSettings["face_delta_min"], System.Globalization.CultureInfo.InvariantCulture);
                    face_delta_max = float.Parse(ConfigurationManager.AppSettings["face_delta_max"], System.Globalization.CultureInfo.InvariantCulture);
                    dpi = int.Parse(ConfigurationManager.AppSettings["ScannerDpi"]);
                }
                catch
                {
                    //Console.WriteLine("Не смог считать параметры face_wigth и.т.д.");
                    logger.Error("Не смог считать параметры face_wigth и.т.д.");
                    face_width = 2.0f;
                    face_height = 2.6f;
                    face_delta_min = 0.2f;
                    face_delta_max = 0.9f;
                    dpi = 300;
                }
                double minRectangleSquare = face_width * face_height * face_delta_min;
                double maxRectamgleSquare = face_width * face_height * face_delta_max;
                originalFrame = new Image<Bgr, Byte>(new Bitmap((Image)inputImage.Clone()));
                inputImage.Save("Crop" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");
                var cfg = string.IsNullOrEmpty(ConfigurationManager.AppSettings["UsePythonForFindFaceInScan"]) ? true : bool.Parse(ConfigurationManager.AppSettings["UsePythonForFindFaceInScan"]);
                if (!cfg)
                {
                    var Result = FindFace(inputImage);
                    int GoodFaces = Result.GoodFacesFound();
                    if (GoodFaces == 1)
                    {
                        //need doc detector before invoke 
                        inputImage = CheckFramePosition(inputImage);
                        AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(device, new List<AnalyzeImageResultType>(), inputImage, Result.faceImage));
                        return;
                    }
                    else
                    {
                        if (GoodFaces == 0)
                        {
                            AnalyzeImage(this, new AnalyzeCompletedEventArgs(device, new List<AnalyzeImageResultType> { AnalyzeImageResultType.FaceNotFound }));
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < GoodFaces; i++)
                            {
                                Result.RList[i].faceImage.Save("GoodFace_" + i + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");
                            }
                            AnalyzeImage(this, new AnalyzeCompletedEventArgs(device, new List<AnalyzeImageResultType> { AnalyzeImageResultType.MoreThanOnePerson }));
                            return;
                        }
                    }
                    return;
                }
                isFaceFound = true;

            } // end scanner logic
            if (isFinalGoodImageFound == true && device == DeviceType.WebCam)
            {   // процесс обработки с камеры закончен
               //Console.WriteLine("Good final image found. We will not process more images.");
                logger.Info("Good final image found.  We will not process more images.");
                return; // ничего делать не надо
            }
            currentFrame = originalFrame;
            
            isFaceFound = true;
            FaceDetectorProcessor fdp = new FaceDetectorProcessor(currentFrame);
            Rectangle[] faceFoundRect = fdp.isFaceDetected();
            if (faceFoundRect != null && faceFoundRect.Length == 1)
            {
                //isFaceFound = true;
                // рисуем рамку лица на экране
                using (Graphics g = Graphics.FromImage(inputImage))
                {

                    Rectangle rec = faceFoundRect[0];

                    var scaleX = getScale(baseImg.Height, inputImage.Height);

                    var scaleY = getScale(baseImg.Width, inputImage.Width);

                    rec.X = Convert.ToInt32(rec.X * scaleX);

                    rec.Y = Convert.ToInt32(rec.Y * scaleY);

                    rec.Height = Convert.ToInt32(rec.Height * scaleY);

                    rec.Width = Convert.ToInt32(rec.Width * scaleX);

                    g.DrawRectangle(new Pen(Color.LimeGreen, 5.0f), rec);

                }

                //inputImage.Save(string.Format("D:\\\\qwe {0}.png", DateTime.Now.Ticks));

            }
            //вежливо попросили мусорщика забрать объект, на который ссылался указатель fdp, т.к. все равно создадим новый объек в памяти, в следующей итерации.
            fdp = null;
            if (faceFoundRect != null && faceFoundRect.Length > 1)
            {
                // у нас слишком много лиц                
                var Error = new List<AnalyzeImageResultType>();
                Error.Add(AnalyzeImageResultType.MoreThanOnePerson);
                AnalyzeImage(this, new AnalyzeCompletedEventArgs(device, Error, baseImg, null));
                //Console.WriteLine("More than 2 faces found in CV in c#!");
                logger.Info("More than 2 faces found in CV in c#!");
                return;
            }
            if (isFaceFound && backgoundThread_ImageAnalizer.IsBusy == false && isFinalGoodImageFound == false)
            {
                var dto = new ImgAnalyzerDto
                {
                    BaseImage = baseImg,
                    Device = device,
                    Img = originalFrame,
                    isCamStopped = imgArgs.СamStopped,
                    ImgFilePath = imgArgs.ImgPath
                };
                logger.Info("START THREAD TO ANALIZE IN PYTHON !");
                backgoundThread_ImageAnalizer.RunWorkerAsync(dto);
            }
            else
            {
                //  кастом логика для webCam
                if (device == DeviceType.WebCam && imgArgs.СamStopped && backgoundThread_ImageAnalizer.IsBusy == false)
                {

                    if (webCamErrors.ToList().Count == 0)

                        webCamErrors.Add(AnalyzeImageResultType.FaceNotFound);

                    AnalyzeCompleted(this, new AnalyzeCompletedEventArgs(device, webCamErrors.ToList(), inputImage, null));

                    return;

                }



                AnalyzeImage(this, new AnalyzeCompletedEventArgs(device, new List<AnalyzeImageResultType>() { AnalyzeImageResultType.FaceNotFound }, inputImage, null));



                if (!webCamErrors.Contains(AnalyzeImageResultType.FaceNotFound))
                {

                    webCamErrors.Add(AnalyzeImageResultType.FaceNotFound);

                }



            }

            //logger.Info("RunParralelFrameGrabber exiting");

        }

        private static Func<int, int, float> getScale = (m1, m2) =>
        {
            if (m1 != m2)
            {
                return m1 / (float)m2;
            }
            return 1;
        };



        /****************************************************************************************************/
        #region Implementation of IImageAnalyzer
        public void Analyze(ImageChangedEventArgs imgArgs, DeviceType device)
        {
            //logger.Info("entering Analyze Method");
            var qwe = AnalyzeCompleted.GetInvocationList();

            Image inputImage = imgArgs.Img;

           //logger.Info("Анализируемый файл: " + imgArgs.ImgPath);

            ProcessNewImageFromDevice(inputImage, device, imgArgs);

            //logger.Info("After run parrallel framegraber. Exiting Analyze");

        }
        
        public event EventHandler<AnalyzeCompletedEventArgs> AnalyzeCompleted = (sender, e) => { };
        public event EventHandler<AnalyzeCompletedEventArgs> AnalyzeImage = (sender, e) => { };

        #endregion
    }
}
