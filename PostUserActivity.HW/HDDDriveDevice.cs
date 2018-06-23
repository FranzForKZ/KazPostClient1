using PostUserActivity.Contracts.HWContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Timer = System.Timers.Timer;
namespace PostUserActivity.HW
{

    public class HDDDriveDevice : IHWDeviceWork
    {
        Timer stopTimer = null;        
        Logger logger = LogManager.GetCurrentClassLogger();
        DirectoryInfo DI;
        bool IsItHddScanner = false;
       // System.Drawing.Image lastImage;
        string PathForFileLoad { get; set; }
        
        public void Start()
        {
            if (IsItHddScanner)
            {
                ImgWithPath obj = GetNextImageFromDisk();
                ImageChanged(this, new ImageChangedEventArgs(obj.Img, obj.Path));
            }
        }

        public void Stop()
        {
            // останавливаем перебор
            isStopped = true;
        }

        private bool isStopped = true;

        public void StartRecord()
        {

            DI = new DirectoryInfo(PathForFileLoad);
            isStopped = false;
            stopTimer = new Timer(durationTimeInSec * 1000);
            stopTimer.Elapsed += stopTimer_Elapsed;
            stopTimer.AutoReset = false;
            stopTimer.Start();
            NextImage();
            //перебираем файлы, как только нашли новый вызываем ImageChanged
        }

        private void stopTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            isStopped = true;
            var obj = GetNextImageFromDisk();
            Completed(this, new HWCompletedEventArgsEventArgs(obj.Img, "Record stopped", true));
        }

        private void NextImage()
        {
            while (!isStopped)
            { 
                //  читай файл, вызывай евент
                //
                ImgWithPath obj = GetNextImageFromDisk();

                   // lastImage = obj.Img;
                    ImageChanged(this, new ImageChangedEventArgs(obj.Img, obj.Path));
                
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(TimeToSleep);
            }
        }
        int Iteration = 0;

        private ImgWithPath GetNextImageFromDisk()
        {
           
            try
            {
                ImgWithPath obj = new ImgWithPath();
                int Count = DI.GetFiles("*.jpg").Count();
                if(Count == 0 )
                {
                    string path = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "EmptyFolder.jpg");
                    obj.Path = path;
                    obj.Img = System.Drawing.Image.FromFile(path);
                    return obj;
                }
                if (Iteration == Count)
                {
                    Iteration = 0;
                }
                FileInfo file = DI.GetFiles("*.jpg")[Iteration];
                obj.Img = System.Drawing.Image.FromFile(file.FullName);
                obj.Path = file.FullName;
                Iteration++;
                //logger.Info("Имя анализируемого файла: "+ file.FullName);
                return obj;
            }
            catch (Exception ex)
            {
                logger.Error("Cant get file from LoadFolder: " + ex.Message);
            }
            return null;
        }


        public bool IsRunning()
        {
            return !isStopped;
        }

        public event EventHandler<HWCompletedEventArgsEventArgs> Completed;

        public event EventHandler<HWErrorEventArgs> Error;

        public event EventHandler<ImageChangedEventArgs> ImageChanged;

        public event EventHandler<ImageChangedEventArgs> PreviewImageChanged;
        private int TimeToSleep;
        /// <summary>
        /// Конструктор устройства "диск"
        /// </summary>
        /// <param name="PathForFileLoad">Папка из которой берем файлы</param>
        public HDDDriveDevice(string PathForFileLoad)
        {
            this.PathForFileLoad = PathForFileLoad;
        }
        /// <summary>
        /// Конструктор устройства диск
        /// </summary>
        /// <param name="PathForFileLoad">Откуда брать фото</param>
        /// <param name="ReadSpeedFromConfig">С какой скорость в кадрах, берется из конфига</param>
        public HDDDriveDevice(int durationTimeInSec,string PathForFileLoad, string ReadSpeedFromConfig)
        {
            this.durationTimeInSec = durationTimeInSec;
            this.PathForFileLoad = PathForFileLoad;
            DI = new DirectoryInfo(PathForFileLoad);
            try
            {
                this.TimeToSleep =(int)(1000 /double.Parse(ReadSpeedFromConfig, System.Globalization.CultureInfo.InvariantCulture));
             
                if(this.TimeToSleep < 64)
                {
                    //чтобы нельзя было задать бесконечное большое число кадров в секунду 
                    this.TimeToSleep = 64;
                }
            }
            catch(Exception ex)
            {
                logger.Error("Ошибка конфига! ImageFromDiskPerSecond отсутствует или задан не верно");
            }
        }
        public HDDDriveDevice(int durationTimeInSec, string PathForFileLoad)
        {
                this.durationTimeInSec = durationTimeInSec;
                this.PathForFileLoad = PathForFileLoad;
                DI = new DirectoryInfo(PathForFileLoad);
                this.TimeToSleep = 5 * 1000;
                this.IsItHddScanner = true;
        }
        public void Dispose()
        {
          
        }



        public int durationTimeInSec { get; set; }


        public IntPtr ParentWindowHandle
        {
            get;
            set;
        }
    }
    public class ImgWithPath
    {
       public System.Drawing.Image Img{get;set;}
        public string Path { get; set; }
    }
}
