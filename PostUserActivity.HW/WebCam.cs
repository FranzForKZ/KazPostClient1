using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AForge.Video.DirectShow;
using AForge.Video;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using AForge.Video.FFMPEG;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.HWContracts;
using NLog;
using Timer = System.Timers.Timer;

namespace PostUserActivity.HW
{
    /// <summary>
    /// класс содержит логику для работы с Web камерами
    /// </summary>
    public class WebCam : IHWDeviceWork
    {
        Timer stopTimer = null;        

        Logger logger = LogManager.GetCurrentClassLogger();
        private VideoCaptureDevice cameraDevice = null;

        //private VideoFileWriter writer = null;

        //private long m_framesReceived = 0L;
        //private long totalFramesCount = 0L;

        private int durationTimeSec = 0;
        private bool isRecording;

        #region implement IHWDeviceWork events

        /// <summary>
        /// у камеры событие будет вызываться с каждым изменением фрейма
        /// </summary>
        public event EventHandler<ImageChangedEventArgs> ImageChanged = (sender, args) => { };

        public event EventHandler<ImageChangedEventArgs> PreviewImageChanged = (sender, args) => { };
        public event EventHandler<HWErrorEventArgs> Error = (sender, args) => { };
        public void Stop()
        {
            if(stopTimer!=null)
            {
                stopTimer.Stop();
                stopTimer.Elapsed -= stopTimer_Elapsed;
               
            }
           
            isRecording = false;
            if (cameraDevice != null)
            {
                cameraDevice.SignalToStop();
            }   
         
        }

        public void StartRecord()
        {            
            if (!cameraDevice.IsRunning)
            {
                cameraDevice.Start();
            }

            isRecording = true;
            stopTimer = new Timer(durationTimeSec * 1000);
            stopTimer.Elapsed += stopTimer_Elapsed;
            stopTimer.AutoReset = false;
            stopTimer.Start();
        }

        public bool IsRunning()
        {
            if (cameraDevice != null)
            {
                return cameraDevice.IsRunning;
            }

            return false;
        }

        public event EventHandler<HWCompletedEventArgsEventArgs> Completed = (sender, args) => { };

        #endregion

        public WebCam(HWDeviceDesciption selectedCamera)
        {
            isRecording = false;
            //logger.Debug("WebCam Constructor");
            cameraDevice = new VideoCaptureDevice(selectedCamera.DeviceId);
            cameraDevice.NewFrame += CameraDevice_NewFrame;
            //logger.Debug("WebCam Constractor end");
        }

        public WebCam(int durationTimeSec, HWDeviceDesciption selectedCamera)
            : this(selectedCamera)
        {           
           // logger.Debug("WebCam Constructor");
            //cameraDevice = new VideoCaptureDevice(selectedCamera.DeviceId);
            //cameraDevice.NewFrame += CameraDevice_NewFrame;
            this.durationTimeSec = durationTimeSec;
           // logger.Debug("WebCam Constractor end");

        }

        void stopTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (cameraDevice.IsRunning)
            {
                cameraDevice.SignalToStop();
                Completed(this, new HWCompletedEventArgsEventArgs(lastImage, "Record stopped", true));
            }

            //stopTimer.Stop();
        }

        public void Start()
        {            
            isRecording = false;
            //logger.Debug("WebCam Start");
            //stopTimer = new Timer(TimerCallback, null, new TimeSpan(0, 0, durationTimeSec), new TimeSpan(0, 0, durationTimeSec));
            //var width = cameraDevice.VideoCapabilities[0].FrameSize.Width;
            //var height = cameraDevice.VideoCapabilities[0].FrameSize.Height;
            //var frameRate = cameraDevice.VideoCapabilities[0].AverageFrameRate;
            //totalFramesCount = Convert.ToInt64(durationTimeSec * frameRate);  //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //m_framesReceived = 0L;
            // create new video file
            //writer = new VideoFileWriter();
            //writer.Open(path, width, height, frameRate, VideoCodec.MPEG4);
            
           // if(cameraDevice.)
            if(cameraDevice.Source != "")
            cameraDevice.Start();


          //  logger.Debug("WebCam Start END");
        }


        #region event handlers

        Image lastImage;

        private void CameraDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
          //  logger.Debug("CameraDevice_NewFrame");

            if(cameraDevice == null)
                return;

            try
            {
                
                //m_framesReceived++;
                // clone the bitmap. why? commented it

                var img = (Image)eventArgs.Frame.Clone();       
                lastImage = (Image) eventArgs.Frame.Clone();
                //var imgCloned = (Image)img.Clone();
                //writer.WriteVideoFrame(img);
                eventArgs.Frame.Dispose();
                //if (m_framesReceived >= totalFramesCount && totalFramesCount != 0L)
                //{
                //    cameraDevice.SignalToStop();
                //    Thread.Sleep(1000);
                //    Completed(this, new HWCompletedEventArgsEventArgs(img, "Record stopped"));
                //    //writer.Close();
                //}
                //else
                //{
                //    ImageChanged(this, new ImageChangedEventArgs(img));
                //}
                if (isRecording)
                {
                    ImageChanged(this, new ImageChangedEventArgs(img));
                }
                else
                {
                    PreviewImageChanged(this, new ImageChangedEventArgs(img));
                }
            }            
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invoke or BeginInvoke cannot be called"))
                {
                    //  do nothing
                }
                else
                {
                    logger.Error(ex);
                    Error(this, new HWErrorEventArgs("Ошибка создания нового кадра"));
                }                
            }

//            logger.Debug("CameraDevice_NewFrame end");
            //  сохраним картинку
            //imgCloned.Save(OutputFileName.Get(outputFolder, recordUID, startedDateTime, Convert.ToInt32(m_framesReceived), "jpg"), ImageFormat.Jpeg);
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            if (cameraDevice != null)
            {
                cameraDevice.NewFrame -= CameraDevice_NewFrame;
                if (cameraDevice.IsRunning)
                {
                    cameraDevice.SignalToStop();
                    //cameraDevice.WaitForStop();
                    //cameraDevice.Stop();
                    cameraDevice = null;
                }
            }            
        }

        #endregion        
    

        public IntPtr ParentWindowHandle
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
