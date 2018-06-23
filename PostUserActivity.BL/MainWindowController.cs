using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using CommonLib;
using PostUserActivity.BL;
using PostUserActivity.BL.States;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.Contracts.Network;
using PostUserActivity.HW;
using AForge.Video.DirectShow;
using PostUserActivity.Tevian;
//using PostUserActivity.Python;

namespace PostUserActivity
{
    /// <summary>
    /// контроллер, который содержит основную логику работы приложения    
    /// </summary>
    public class MainWindowController : IMainWindowController
    {

        public int MAX_RETRY_COUNT = int.MaxValue;
        private bool allowrescan = false;
        private int scanner_dpi = 0;
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #region fields & properties

        private readonly string PythonPath;

        private IImageAnalyzer ImageAnalyzer;

        public Rectangle FaceCoodinate { get; set; }

        private IDeviceConfiguration Configuration;

        public int ScanRetry { get; private set; }
        public int CamRetry { get; private set; }

        public void StopWebCamPreview()
        {
            webCam.Stop();
        }

        public int MaxReryCount
        {
            get { return MAX_RETRY_COUNT; }
        }

        private DeviceWorkFactory devicesFactory;
        protected GetPictureDialogFactory ImgDlg;

        private IHWDeviceWork scanner;
        private IHWDeviceWork webCam;


        private object webCamLockObject = new object();
        public AnalyzeCompletedEventArgs ScannerAnalyzeResult { get; private set; }

        // результаты работы камеры
        public AnalyzeCompletedEventArgs WebCamAnalyzeResult { get; private set; }


        public DeviceStateBase ScannerState { get; private set; }
        public DeviceStateBase WebCamState { get; private set; }

        public string MediaDirectory { get; private set; }
        public bool IsFinished { get; private set; }

        public bool IsWebResultNull()
        {
            if (WebCamAnalyzeResult == null)
                return true;
            else
                return false;
        }
        private AnalyzeImgArgs TaskImageIdents;

        #endregion

        #region HW events

        public event EventHandler<ImageChangedEventArgs> ScannerImageChanged = (sender, e) => { };
        //public event EventHandler<HWCompletedEventArgsEventArgs> ScannerCompleted = (sender, e) => { };
        //public event EventHandler<HWErrorEventArgs> ScannerError = (sender, e) => { };
        public event EventHandler<ImageChangedEventArgs> WebCamImageChanged = (sender, e) => { };
        //public event EventHandler<HWCompletedEventArgsEventArgs> WebCamCompleted = (sender, e) => { };
        //public event EventHandler<HWErrorEventArgs> WebCamError = (sender, e) => { };

        #endregion

        #region events

        public event EventHandler<AnalyzeCompletedEventArgs> ScannerImageAnalyzeCompleted = (sender, e) => { };
        public event EventHandler<AnalyzeCompletedEventArgs> WebCamImageAnalyzeCompleted = (sender, e) => { };

        public event EventHandler<DoTakeRetryEvent> DoTakePhotoEvent = (sender, e) => { };
        public event EventHandler<DoTakeRetryEvent> DoTakeScanEvent = (sender, e) => { };

        public event EventHandler<AnalyzeCompletedEventArgs> WebCamProcessErrors = (sender, e) => { };
        public event EventHandler<AnalyzeCompletedEventArgs> ScanProcessErrors = (sender, e) => { };

        #endregion
        private MainWindowController(IDeviceConfiguration configuration)
        {
            this.Configuration = configuration;
            try
            {
                MAX_RETRY_COUNT = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxRetry"]);
                scanner_dpi = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ScannerDpi"]);
                if (System.Configuration.ConfigurationManager.AppSettings["AllowReScan"] == "1")
                {
                    this.AllowReScan = true;
                }
                else
                {
                    this.AllowReScan = false;
                }

            }
            catch (Exception ex)
            {

                logger.Error("Ошибка параметров приложения. Проверьте правильность ввода MaxRetry, AllowReScan. Error message : \n " + ex);
                Console.WriteLine("Ошибка параметров app.config. Проверь имя и значение");
            }

            ScannerState = new ScanStartState(this);
            ScannerState = ScannerState.ChangeState();

            //  проверка на то, что файлы или камера

            WebCamState = new WebCamStartState(this);
            WebCamState = WebCamState.ChangeState();

            //            webCam = new HDDDriveDevice("path to folder");


            MediaDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MediaFolder");
            if (!Directory.Exists(MediaDirectory))
            {
                Directory.CreateDirectory(MediaDirectory);
            }
        }
        public void UpdateRetry(DeviceType device)
        {
            switch (device)
	        {
                case DeviceType.WebCam:
                    CamRetry++;
                break;
                case DeviceType.Scanner:
                    ScanRetry++;
                break;
                default:
                break;
	        }
        }
        public bool IsWebCamAvailible()
        {
            var device = Configuration.GetDevice(DeviceType.WebCam);
            var videoDevices = new AForge.Video.DirectShow.FilterInfoCollection(FilterCategory.VideoInputDevice);
            //если запускаем в первый раз, то можно заодно сохранить первую попавшуюся камеру, если такая есть..
            if (device.Name == "")
            {
                if (videoDevices.Count != 0)
                {
                    Configuration.SaveDevice(new HWDeviceDesciption
                   {
                       Device = DeviceType.WebCam,
                       Name = videoDevices[0].Name,
                       DeviceId = videoDevices[0].MonikerString
                   });


                }
                return true;
            }
            for (int i = 0; i < videoDevices.Count; i++)
            {

                if (device.Name == videoDevices[i].Name)
                {
                    if (device.DeviceId == videoDevices[i].MonikerString)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CheckCompleted { get; set; }
        private bool IsScanerWork { get; set; }
        public bool IsScannerAvailible()
        {

            var Scanner = Configuration.GetDevice(DeviceType.Scanner);
            var twain = new Saraff.Twain.Twain32();
            var isDsmOpened = false;
            try
            {
                twain.IsTwain2Enable = false;
                isDsmOpened = twain.OpenDSM();
                twain.ShowUI = false;

            }
            catch (Exception ex)
            {
                if (twain != null)
                {
                    twain.CloseDSM();
                    twain.Dispose();
                }
                logger.Error(ex);
                return false;
            }
            if (isDsmOpened)
            {
                if (!string.IsNullOrEmpty(Scanner.DeviceId))
                {
                    if (twain.SourcesCount >= int.Parse(Scanner.DeviceId) + 1)
                    {
                        twain.SourceIndex = int.Parse(Scanner.DeviceId);
                    }
                    else
                    {
                        return false;
                    }

                }
                else //если мы все-таки папали в ветку else, значит устройства еще не было, нужно сохранить первый попавшийся в конфиг
                {
                    if (twain.SourcesCount != 0)
                    {
                        Scanner = new HWDeviceDesciption
                        {
                            Device = DeviceType.Scanner,
                            DeviceId = "0",
                            Name = twain.GetSourceProductName(0)
                        };
                        Configuration.SaveDevice(Scanner);
                        twain.SourceIndex = 0;
                    }
                }
                for (int i = 0; i < twain.SourcesCount; i++)
                {
                    if (Scanner.Name == twain.GetSourceProductName(i))
                    {
                        //совпадение по имени выбранного устройства со списком не гарантирует работу сканера. сканер может быть выключен или иметь битый драйвер, например.
                        if (Scanner.DeviceId == i.ToString())
                        {
                            //проверить работу сканера можно здесь...
                            //неужели начать сканирование и оборвать единственный способ? 
                            twain.AcquireError += twain_AcquireError;
                            twain.AcquireCompleted += twain_AcquireCompleted;
                            try
                            {
                                if (twain.SourcesCount >= int.Parse(Scanner.DeviceId) + 1)
                                {
                                    twain.OpenDataSource();
                                    var resolutions = twain.Capabilities.XResolution.Get();
                                    if (resolutions.Count > 0)
                                    {
                                        twain.Capabilities.XResolution.Set((float)resolutions[0]);
                                        twain.Capabilities.YResolution.Set((float)resolutions[0]);
                                    }
                                    this.CheckCompleted = false;
                                    twain.ImageLayout = new RectangleF(new PointF(0, 0), new SizeF(0.2f, 0.2f));
                                    twain.ShowUI = false;
                                    twain.Acquire();
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            catch (Exception ex)
                            {
                                twain.CloseDataSource();
                                twain.CloseDSM();
                                twain.Dispose();
                                logger.Error("Не могу установить соединение со сканером: \n" + ex.Message + "\n Inner exeption: \n" + ex.InnerException);
                                return false;
                            }
                            while (!this.CheckCompleted)
                            {
                                System.Windows.Forms.Application.DoEvents();
                            }
                            return this.IsScanerWork;
                        }
                    }
                }
                // если мы дошли до этого места, значит:
                // 1. в конфиге уже сохранено какое-то устройство.
                // 2. в списке доступных устройств его нет
                // по идеи, ничего делать не нужно, возвращаем false. выбранное устройство не доступно(нет в списке)

                //
            }
            return false;
        }
        void twain_AcquireCompleted(object sender, EventArgs e)
        {
            this.CheckCompleted = true;
            this.IsScanerWork = true;
            var twain = (Saraff.Twain.Twain32)sender;
            twain.CloseDataSource();
            twain.CloseDSM();
            twain.Dispose();
        }

        void twain_AcquireError(object sender, Saraff.Twain.Twain32.AcquireErrorEventArgs e)
        {
            this.CheckCompleted = true;
            this.IsScanerWork = false;
            //if error was here
        }
        public int GetScannerDpi()
        {
            return scanner_dpi;
        }

        public bool AllowReScan
        {
            get
            {
                return allowrescan;
            }
            private set { allowrescan = value; }

        }
        public MainWindowController(AnalyzeImgArgs imgIdent, IDeviceConfiguration configuration)
            : this(configuration)
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            this.PythonPath = System.Configuration.ConfigurationManager.AppSettings["PythonDetectorPath"];

            this.TaskImageIdents = imgIdent;

            //this.ImageAnalyzer = new ImageAnalyzer(PythonPath);
            //this.ImageAnalyzer.AnalyzeCompleted += ImageAnalyzer_AnalyzeCompleted;
            //this.ImageAnalyzer.AnalyzeImage += ImageAnalyzer_AnalyzeImage;

            this.ImgDlg = new GetPictureDialogFactory(configuration);

            devicesFactory = new DeviceWorkFactory(configuration);

            ChangeDefaultScanner();
            ChangeDefaultWebCam();
        }


        public void SetScanAttempts(int Attempts)
        {
            ScanRetry = Attempts;

        }
        public void SetCamAttempts(int Attempts)
        {
            CamRetry = Attempts;

        }
        public int GetScanAttempts()
        {
            return ScanRetry;
        }

        public int GetCamAttempts()
        {
            return CamRetry;
        }

        private bool IsScanReadyToSend(int maxAttempts)
        {
            //if (maxAttempts == 0)
            //{
            //    if ((ScannerAnalyzeResult != null && !ScannerAnalyzeResult.ErrorsList.Any()))
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //получили результат хотя бы раз
            if ( maxAttempts != 0 && maxAttempts <= ScanRetry)
            {
                return true;
            }   
                if (ScannerAnalyzeResult != null) //&& !ScannerAnalyzeResult.ErrorsList.Any()) //ScanRetry == maxAttempts ||
                {
                    // есть что отправлять
                    if (ScannerAnalyzeResult.FullImage != null)
                    {
                        return true;
                    }
                }    
            return false;
        }

        private bool IsCamReadyToSend(int maxAttempts)
        {
            //if (maxAttempts == 0)
            //{
            //    if ((WebCamAnalyzeResult != null && !WebCamAnalyzeResult.ErrorsList.Any()))
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //получили результат хотя бы раз
            if (maxAttempts != 0 && maxAttempts <= CamRetry)
            {
                return true;
            }   
            if(WebCamAnalyzeResult!= null) //&& !WebCamAnalyzeResult.ErrorsList.Any()) //CamRetry == maxAttempts ||
                {
                   // есть что отправлять
                    if (WebCamAnalyzeResult.FullImage != null)
                    {
                        return true;
                    }
                    
                }
            //}
            return false;
        }

        public bool CanCreatePacket()
        {
            if (IsScanReadyToSend(MAX_RETRY_COUNT) && IsCamReadyToSend(MAX_RETRY_COUNT))
            {
                return true;
            }
            return false;
        }

        public bool CanScan()
        {
            if (MaxReryCount == 0)
            {
                return true;
            }
            if (ScanRetry >= MAX_RETRY_COUNT)
            {
                return false;
            }
            return true;
        }

        public bool CanCam()
        {
            if (MaxReryCount == 0)
            {
                return true;
            }
            if (CamRetry >= MAX_RETRY_COUNT)
            {
                return false;
            }
            return true;
        }



        private void ImageAnalyzer_AnalyzeImage(object sender, AnalyzeCompletedEventArgs e)
        {
            switch (e.SourceDevice)
            {
                case DeviceType.WebCam:
                    if (WebCamState.ErrorsList == null)
                    {
                        WebCamState.ErrorsList = new List<AnalyzeImageResultType>();
                    }
                    WebCamState.ErrorsList.AddRange(e.ErrorsList);
                    WebCamState.ErrorsList = WebCamState.ErrorsList.Distinct().ToList();
                    WebCamState.ErrorMessage = e.Message;
                    //ChangeWebCamState();
                    //WebCamProcessErrors(this, e);

                    return;
                case DeviceType.Scanner:
                    ScannerState.ErrorsList = e.ErrorsList;
                    ScannerState.ErrorMessage = e.Message;
                    SaveImages(e);
                    ChangeScannerState();
                    ScanProcessErrors(this, e);
                    return;
            }
        }

        private void ImageAnalyzer_AnalyzeCompleted(object sender, AnalyzeCompletedEventArgs e)
        {
            //this.ImageAnalyzer = new ImageAnalyzer(PythonPath);
            //this.ImageAnalyzer.AnalyzeCompleted -= ImageAnalyzer_AnalyzeCompleted;
            //this.ImageAnalyzer.AnalyzeImage -= ImageAnalyzer_AnalyzeImage;

            // мы нашли подходящее изображение
            switch (e.SourceDevice)
            {
                case DeviceType.Scanner:
                    //if (ScannerAnalyzeResult == null)
                    //{

                    if (e.IsSuccess())
                    {
                        ScannerAnalyzeResult = e;
                        SaveImages(e);
                        
                    }
                    else
                    {
                        //UpdateRetry(DeviceType.Scanner);
                        //не отсылаем последнее удачное фото на главную форму...
                        //if (ScannerAnalyzeResult != null) //если есть чего возращать, давайте вернем..
                        //{
                        //    ChangeScannerState();
                        //    AnalyzeCompletedEventArgs LastSuccess = new AnalyzeCompletedEventArgs(e.SourceDevice, e.ErrorsList, ScannerAnalyzeResult.FullImage, ScannerAnalyzeResult.PreviewImage);
                        //    ScannerImageAnalyzeCompleted(this, LastSuccess);
                        //    break;
                        //}
                       
                       
                    }
                        ChangeScannerState();
                        ScannerImageAnalyzeCompleted(this, e);
                        //e.FullImage.Save(DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");
                    //}

                    break;
                case DeviceType.WebCam:

                    //if (this.WebCamAnalyzeResult == null)
                    //    Console.WriteLine("ImageAnalyzer_AnalyzeCompleted webCam entering NULL");
                    //else
                    //    Console.WriteLine("ImageAnalyzer_AnalyzeCompleted webCam entering " + WebCamAnalyzeResult.ToString() + "  " + WebCamAnalyzeResult.IsSuccess().ToString());

                    //if (WebCamAnalyzeResult == null || WebCamAnalyzeResult.IsSuccess() == false)
                    //{
                    //    lock (webCamLockObject)
                    //    {
                    //        if (WebCamAnalyzeResult == null || WebCamAnalyzeResult.IsSuccess() == false)
                    //        {
                    webCam.Stop();
                    //создадим в куче копию найденных изображений
                    if (e.IsSuccess())
                    {
                        WebCamAnalyzeResult = e; //получаем результаты работы камеры и СОХРАНЯЕМ ИХ
                        SaveImages(e);
                    }
                    else
                    {
                        //UpdateRetry(DeviceType.WebCam);
                        // if (WebCamAnalyzeResult != null) //если есть чего возращать, давайте вернем..
                        //{
                        //    ChangeWebCamState();
                        //    AnalyzeCompletedEventArgs LastSuccess = new AnalyzeCompletedEventArgs(e.SourceDevice, e.ErrorsList, WebCamAnalyzeResult.FullImage, WebCamAnalyzeResult.PreviewImage);
                        //    WebCamImageAnalyzeCompleted(this, LastSuccess);
                        //    break;
                        //}
                    }
                        ChangeWebCamState();
                        WebCamImageAnalyzeCompleted(this, e);
                    //if (WebCamState.ErrorsList == null)
                    //{
                    //    WebCamState.ErrorsList = new List<AnalyzeImageResultType>();
                    //}

                    //if (e.ErrorsList != null)
                    //{
                    //    WebCamState.ErrorsList.AddRange(e.ErrorsList);
                    //    WebCamState.ErrorsList = WebCamState.ErrorsList.Distinct().ToList();
                    //}

                   

                    var Analyser = (Tevian.Analyser)ImageAnalyzer;
                    Analyser.StopCamAnalyze();
                    //        }
                    //    }
                    //}
                    break;
            }
        }

        private void SaveImages(AnalyzeCompletedEventArgs e, string postfix = "")
        {
            //  save images
            var dt = DateTime.Now;
            postfix = string.IsNullOrEmpty(postfix) ? "" : "_" + postfix;
            var scFileNamePreview = string.Format("{4}Image_{0}_{1}_{2}_{3}{5}.png", dt.ToString("yyyy-MM-dd_HH_mm_ss"), TaskImageIdents.WmfId, TaskImageIdents.IIN.GetHashCode(), "preview", e.SourceDevice, postfix);
            var scFileNameFull = string.Format("{4}Image_{0}_{1}_{2}_{3}{5}.png", dt.ToString("yyyy-MM-dd_HH_mm_ss"), TaskImageIdents.WmfId, TaskImageIdents.IIN.GetHashCode(), "fullframe", e.SourceDevice, postfix);
            if (e.IsSuccess())
            {
                e.FullImage.Save(Path.Combine(MediaDirectory, scFileNameFull));
                e.PreviewImage.Save(Path.Combine(MediaDirectory, scFileNamePreview));
            }    
           
        }


        private void WebCam_Completed(object sender, HWCompletedEventArgsEventArgs e)
        {
            //  если произошло это действие, значит камера закончила работу            
            var args = new ImageChangedEventArgs(e.Image, true);

            //Нужно для отрисовки лица


            ImageAnalyzer.Analyze(args, DeviceType.WebCam);

            var analyser = (Analyser)ImageAnalyzer;
            if (analyser.FaceCoordinates != null)
            {
                FaceCoodinate = analyser.FaceCoordinates;
            }
        }

        private void Scanner_Error(object sender, HWErrorEventArgs e)
        {
            var errors = new List<AnalyzeImageResultType>
                            {
                AnalyzeImageResultType.ScannerError
            };
            var eArgs = new AnalyzeCompletedEventArgs(DeviceType.Scanner, errors)
            {
                Message = e.Message
            };

            ScannerState.ErrorsList = errors;
            ScannerState.ErrorMessage = e.Message;
            ScannerState = ScannerState.ChangeState();

            ScanProcessErrors(this, eArgs);
        }

        #region Implementation of IMainWindowController


        public IDeviceConfiguration GetConfiguration()
        {
            return Configuration;
        }

        public string GetUserName()
        {
            return this.TaskImageIdents.Username;
        }
        public long GetWfprocessid()
        {
            return this.TaskImageIdents.WmfId;
        }

        public string GetScannerName()
        {
            scanner.ImageChanged -= Scanner_ImageChanged;
            scanner.Error -= Scanner_Error;
            //scanner.Completed -= Scanner_Completed;
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            string ScanJpgPath = System.Configuration.ConfigurationManager.AppSettings["ScanJpgPath"];
            if (!string.IsNullOrEmpty(ScanJpgPath))
            {
                scanner = (IHWDeviceWork)new HDDDriveDevice(5, ScanJpgPath);
            }
            else
            {
                scanner = devicesFactory.GetDeviceWorkFactory(DeviceType.Scanner);
            }
            scanner.ImageChanged += Scanner_ImageChanged;
            scanner.Error += Scanner_Error;
            //scanner.Completed += Scanner_Completed;


            return Configuration.GetDevice(DeviceType.Scanner).Name;
        }
        private IHWDeviceWork CreateCameraOrHDDriveDevice()
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            var PathForImages = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"]) ? "" : System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"];
            if (PathForImages != string.Empty)
            {
                return devicesFactory.GetDeviceWorkFactory(DeviceType.HDDrive);
            }
            else
            {
                return devicesFactory.GetDeviceWorkFactory(DeviceType.WebCam);
            }
        }
        public string GetWebCamName()
        {
            webCam.ImageChanged -= WebCam_ImageChanged;
            webCam.PreviewImageChanged -= WebCamPreview_ImageChanged;
            //webCam.Error -= WebCam_Error;
            webCam.Completed -= WebCam_Completed;
            webCam = CreateCameraOrHDDriveDevice();
            webCam.ImageChanged += WebCam_ImageChanged;
            webCam.PreviewImageChanged += WebCamPreview_ImageChanged;
            //webCam.Error += WebCam_Error;
            webCam.Completed += WebCam_Completed;

            return Configuration.GetDevice(DeviceType.WebCam).Name;
        }

        public void TakeScan()
        {

            //ImageAnalyzer = new ImageAnalyzer(PythonPath, (ConfigurationSettings)Configuration);
            //ImageAnalyzer = new PostUserActivity.Tevian.Analyser();
            //ImageAnalyzer.AnalyzeCompleted += ImageAnalyzer_AnalyzeCompleted;
            //ImageAnalyzer.AnalyzeImage += ImageAnalyzer_AnalyzeImage;

            ChangeScannerState(new ScanStartState(this, true));
            ScannerAnalyzeResult = null; //если не очищать результат, то по идеи в нем будет всегда хранится последний, удачный...или нет так?

            ScanRetry++;
            if (ScannerState.State != ProcessStateType.Error)
            {
                scanner.ParentWindowHandle = this.WindowHandle;
                scanner.Start();
            }
            DoTakeScanEvent(this, new DoTakeRetryEvent(ScanRetry));
        }
        public void TakePhoto()
        {
            //нужно очистить положение рамки и ее размер перед новым сканированием. иначе получим рамку на месте, где фактически уже нет лица при повторном сканировании

            logger.Info("Entering Take Photo");
            var Analyser = (Tevian.Analyser)ImageAnalyzer;
            Analyser.StartCamAnalyze();
            //питон погибнет здесь
            //ImageAnalyzer = new ImageAnalyzer(PythonPath, (ConfigurationSettings)Configuration, webCam);

            //ImageAnalyzer = new PostUserActivity.Tevian.Analyser();
            //ImageAnalyzer.AnalyzeCompleted += ImageAnalyzer_AnalyzeCompleted;
            //ImageAnalyzer.AnalyzeImage += ImageAnalyzer_AnalyzeImage;
            ChangeWebCamState(new WebCamStartState(this, true));

            WebCamAnalyzeResult = null; //если не очищать результат, то по идеи в нем будет всегда хранится последний, удачный...или нет так?


            //  stop            
            CamRetry++;
            if (WebCamState.State != ProcessStateType.Error)
            {

                webCam.StartRecord();
            }
            // start {00:00:00.3330000}

            DoTakePhotoEvent(this, new DoTakeRetryEvent(CamRetry));
            logger.Info("TakePhoto completed");
        }

        #endregion


        #region events methods

        private void Scanner_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            //string FileOnDisk = System.Configuration.ConfigurationManager.AppSettings["ScanJpgPath"];
            //if(string.IsNullOrEmpty(FileOnDisk))
            //{
            ImageAnalyzer.Analyze(e, DeviceType.Scanner);
            ScannerImageChanged(this, e);
            //}
            //else
            //{
            //    if(File.Exists(e.ImgPath))
            //    {
            //        if(Path.GetExtension(e.ImgPath) == ".jpg")
            //        {
            //            var NewArgs = new ImageChangedEventArgs(e.Img, e.ImgPath);
            //            ImageAnalyzer.Analyze(NewArgs, DeviceType.Scanner);
            //            ScannerImageChanged(this, NewArgs);
            //        }
            //        else
            //        {
            //            logger.Info("Файл, указанный в конфиге не является jpg");
            //        }
            //    }
            //    else
            //    {
            //        logger.Info("Файл, указанный в конфиге открыт для записи или не существет.");
            //    }
            //}


        }


        /// <summary>
        /// We got new image from webcam and we are sending it to the processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //string LoadPath = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"]) ? "" : System.Configuration.ConfigurationManager.AppSettings["FolderForLoadingImages"];
        //int Iteration = 0;
        private void WebCam_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            var analyser = (Analyser)ImageAnalyzer;
            if (analyser.FaceCoordinates != null)
            {
                FaceCoodinate = analyser.FaceCoordinates;
            }
            //if (LoadPath != string.Empty)
            //{
            //    var temp = ImgLoading();
            //    ImageChangedEventArgs NewArgs = new ImageChangedEventArgs(temp.Img);
            //    //NewArgs.Img.Save()
            //    NewArgs.ImgPath = temp.Path;
            //    ImageAnalyzer.Analyze(NewArgs, DeviceType.WebCam);
            //    WebCamImageChanged(this, NewArgs);
            //}
            //else

            //{
            if (e.Img != null)
            {
                ImageChangedEventArgs ArgsForAnalyze = new ImageChangedEventArgs((Image)e.Img.Clone(), e.СamStopped);
                ImageAnalyzer.Analyze(ArgsForAnalyze, DeviceType.WebCam);

                //Нужно для отрисовки лица


                WebCamImageChanged(this, e);
            }
            else
            {
                logger.Info("Пришло битое изображение с камеры...Пропускаем");
            }



            //}

        }

        #endregion

        public void loadImageProcessor()
        {
            ImageAnalyzer = new PostUserActivity.Tevian.Analyser();
            ImageAnalyzer.AnalyzeCompleted += ImageAnalyzer_AnalyzeCompleted;
            ImageAnalyzer.AnalyzeImage += ImageAnalyzer_AnalyzeImage;
        }
        public void UnLoadImageProcessor()
        {
        }
        private string FullImgToBase64(AnalyzeCompletedEventArgs args)
        {
            string result = string.Empty;
            if (args != null && args.FullImage != null)
            {
                result = args.FullImage.ToBase64();
            }

            return result;
        }
        public string ArchiveToBase64(string Path)
        {
            try
            {
                var FileContent = File.ReadAllBytes(Path);
                return Convert.ToBase64String(FileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex.Message);

            }
            return string.Empty;


        }
        private string PreviewImgToBase64(AnalyzeCompletedEventArgs args)
        {
            string result = string.Empty;
            if (args != null && args.PreviewImage != null)
            {
                result = args.PreviewImage.ToBase64();
            }

            return result;
        }
        /// <summary>
        /// Создает пакет для отправки из архива
        /// </summary>
        /// <param name="LogName">Имя архива</param>
        /// <param name="Path">Путь к архиву</param>
        /// <param name="timestamp">Время нажатия кнопки перед созданием архивов(общее)</param>
        /// <returns></returns>
        public LogPackage CreateLogPacket(string LogName, string Path, DateTime timestamp)
        {
            var LogPacket = new LogPackage
            {
                Token = HashFunction.Get(PrivateKey.SECRET_WORD),
                WFMId = TaskImageIdents.WmfId,
                IIN = TaskImageIdents.IIN,
                UserName = TaskImageIdents.Username,
                WorkStation = Environment.MachineName,
                TimeStamp = timestamp,
                Type = ArmDataPackageType.Log,
                FileName = LogName,
                FileContent = ArchiveToBase64(Path)
            };
            return LogPacket;
        }
        public List<ArmDataPackage> CreatePackets()
        {
            var result = new List<ArmDataPackage>();
            var timestamp = DateTime.Now;
            var preview = new ArmDataPackage
            {
                Type = ArmDataPackageType.Preview,
                Token = HashFunction.Get(PrivateKey.SECRET_WORD),
                UserName = TaskImageIdents.Username,
                IIN = TaskImageIdents.IIN,
                Timestamp = timestamp,
                WFMId = TaskImageIdents.WmfId,
                Comment = "empty comment",
                CameraPicture = PreviewImgToBase64(WebCamAnalyzeResult),
                ScanPicture = PreviewImgToBase64(ScannerAnalyzeResult)
            };

            //SaveImages(ScannerAnalyzeResult, "before_send");
            //SaveImages(WebCamAnalyzeResult, "before_send");

            //WebCamAnalyzeResult.FullImage.Save("InPaketwebcamFul" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");
            //WebCamAnalyzeResult.PreviewImage.Save("InPaketwebcamprev" +  DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");


            //ScannerAnalyzeResult.FullImage.Save("InPaketscanImgFull" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");
            //ScannerAnalyzeResult.PreviewImage.Save("InPaketscanImPrev" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".jpg");


            var fullFrame = new ArmDataPackage
            {
                Type = ArmDataPackageType.FullFrame,
                Token = HashFunction.Get(PrivateKey.SECRET_WORD),
                UserName = TaskImageIdents.Username,
                IIN = TaskImageIdents.IIN,
                Timestamp = timestamp,
                WFMId = TaskImageIdents.WmfId,
                Comment = "empty comment",
                CameraPicture = FullImgToBase64(WebCamAnalyzeResult),
                ScanPicture = FullImgToBase64(ScannerAnalyzeResult)
            };
            result.Add(preview);
            result.Add(fullFrame);


            return result;

        }

        public DeviceStateBase GetScannerState()
        {
            return ScannerState;
        }

        public DeviceStateBase GetWebCamState()
        {
            return WebCamState;
        }

        public void ChangeWebCamState()
        {
            WebCamState = WebCamState.ChangeState();
        }

        public void ChangeScannerState()
        {
            ScannerState = ScannerState.ChangeState();
        }

        public void ChangeWebCamState(DeviceStateBase state)
        {
            WebCamState = state.ChangeState();
        }

        public void ChangeScannerState(DeviceStateBase state)
        {
            devicesFactory = new DeviceWorkFactory(Configuration);

            ScannerState = state.ChangeState();
        }

        public void ChangeDefaultScanner()
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            //  TODO: перенести создание в другое место
            string ScanJpgPath = System.Configuration.ConfigurationManager.AppSettings["ScanJpgPath"];
            if (!string.IsNullOrEmpty(ScanJpgPath))
            {
                scanner = (IHWDeviceWork)new HDDDriveDevice(5, ScanJpgPath);
            }
            else
            {
                scanner = devicesFactory.GetDeviceWorkFactory(DeviceType.Scanner);
            }

            scanner.ImageChanged += Scanner_ImageChanged;

            scanner.Error += Scanner_Error;
            //scanner.Completed += Scanner_Completed;
        }

        public void ChangeDefaultWebCam()
        {

            webCam = CreateCameraOrHDDriveDevice();
            webCam.ImageChanged += WebCam_ImageChanged;
            webCam.PreviewImageChanged += WebCamPreview_ImageChanged;
            //webCam.Error += WebCam_Error;
            webCam.Completed += WebCam_Completed;
        }


        public void StartWebCamPreview()
        {
            webCam.Start();
        }

        private void WebCamPreview_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            WebCamImageChanged(sender, e);

        }


        #region not using

        #region HW event handlers



        //private void Scanner_Completed(object sender, HWCompletedEventArgsEventArgs e)
        //{

        //    ScannerCompleted(this, e);
        //}

        //private void WebCam_Error(object sender, HWErrorEventArgs e)
        //{
        //    WebCamError(this, e);            
        //}



        #endregion

        #endregion


        #region Implementation of IDisposable

        public void Dispose()
        {
            if (webCam != null)
            {
                webCam.ImageChanged -= WebCam_ImageChanged;
                webCam.PreviewImageChanged -= WebCamPreview_ImageChanged;

                webCam.Dispose();
            }
            if (scanner != null)
            {
                scanner.Dispose();
            }
        }

        #endregion


        public IntPtr WindowHandle { get; set; }
    }
}
