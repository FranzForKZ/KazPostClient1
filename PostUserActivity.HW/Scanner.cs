using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Saraff.Twain;
using System.Drawing.Imaging;
using System.IO;
using PostUserActivity.Contracts.HWContracts;
using System.Configuration;
using NLog;
using System.Windows.Interop;
using System.Windows.Forms;

namespace PostUserActivity.HW
{



    /// <summary>
    /// класс содержит логику для работы со сканерами
    /// </summary>
    public class Scanner : IHWDeviceWork
    {
        #region implement IHWDeviceWork events

        public event EventHandler<HWErrorEventArgs> Error = (sender, args) => { };

        public void Stop()
        {
            //  do nothing
        }

        public void StartRecord()
        {
            //  все ок, у сканера вызова этого метода не должно происходить
            throw new NotImplementedException();
        }

        public bool IsRunning()
        {
            //  все ок, у сканера вызова этого метода не должно происходить
            throw new NotImplementedException();
        }

        public bool IsLayoutSet { get; private set; }

        public event EventHandler<HWCompletedEventArgsEventArgs> Completed = (sender, args) => { };

        /// <summary>
        /// у сканера событие сработает 1 раз, когда будет отсканирована картинка
        /// </summary>
        public event EventHandler<ImageChangedEventArgs> ImageChanged = (sender, args) => { };

        public event EventHandler<ImageChangedEventArgs> PreviewImageChanged = (sender, args) => { };

        #endregion


        Logger logger = LogManager.GetCurrentClassLogger();

        private Twain32 twain;

        private HWDeviceDesciption selectedScanner;

        public Scanner(HWDeviceDesciption selectedScanner)
        {
            this.selectedScanner = selectedScanner;
            twain = new Twain32();
            twain.IsTwain2Enable = true;
            //  set twain settings
            twain.DisableAfterAcquire = true;
            twain.ShowUI = false;
            if (this.ParentWindowHandle != IntPtr.Zero)
            {
                var parentWnd = new NativeWindow();
                parentWnd.AssignHandle(this.ParentWindowHandle);
                twain.Parent = parentWnd;
            }

            // add event handlers
            twain.EndXfer += endXferEventHandler;
            twain.AcquireCompleted += acquireCompletedEventHandler;
            twain.AcquireError += acquireErrorEventHandler;
        }


        /// <summary>
        /// start scanning
        /// </summary>
        public void Start()
        {
            //logger.Debug("Start");
            try
            {
                if (!twain.OpenDSM())
                {
                    logger.Debug("Не могу найти сканер");
                    Error(this, new HWErrorEventArgs("Не могу найти сканер"));
                    return;
                }
                if (!twain.IsTwain2Supported)
                {
                    twain.CloseDSM();
                    twain.IsTwain2Enable = false;
                    twain.OpenDSM();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //  TODO: add logging
                Error(this, new HWErrorEventArgs(ex.Message));
                return;
            }
            if (selectedScanner.DeviceId != "")
                twain.SourceIndex = int.Parse(selectedScanner.DeviceId);
            else
                logger.Debug("selectedScanner.DeviceId == String.Empty ");
            try
            {
                if (!twain.OpenDataSource())
                {
                    logger.Debug("Не могу установить соединение со сканером");
                    Error(this, new HWErrorEventArgs("Не могу установить соединение со сканером"));
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //  TODO: add logging
                Error(this, new HWErrorEventArgs(ex.Message));
                return;
            }

            int dpiResolution = 200;

            if (System.Configuration.ConfigurationManager.AppSettings["ScannerDpi"] != null)
                dpiResolution = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ScannerDpi"]);



            setPixelType();
            float Height, Weight;
            int X, Y;
            //ScanWidht
            try
            {
                Height = float.Parse(System.Configuration.ConfigurationManager.AppSettings["ScanHeight"], System.Globalization.CultureInfo.InvariantCulture) / (float)2.54;
                Weight = float.Parse(System.Configuration.ConfigurationManager.AppSettings["ScanWidht"], System.Globalization.CultureInfo.InvariantCulture) / (float)2.54;
                X = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ScanStartPointX"]);
                Y = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ScanStartPointY"]);
            }
            catch (Exception ex)
            {
                Height = 6.6462525f;
                Weight = 6.6462525f;
                X = 0;
                Y = 0;
                logger.Error(ex);
            }

            //задаем область сканирования в точке 0 и размером A6
            IsLayoutSet = SetImageLayOut(new RectangleF(new PointF(X, Y), new SizeF(Weight, Height)));
            //если не смогли задать область сканируем не выше 300 dpi
            if (IsLayoutSet)
            {
                SetResolutions(dpiResolution);
            }
            else
            {
                SetResolutions(300);
            }

            try
            {
                twain.Acquire();
            }
            catch (Exception ex)
            {
                twain.CloseDSM();
                twain.CloseDataSource();

                logger.Error(ex);
                //  TODO: add logging
                Error(this, new HWErrorEventArgs(ex.Message));
                return;
            }

        }


        public bool SetImageLayOut(RectangleF NewImgLayout)
        {
            try
            {
                this.twain.ImageLayout = NewImgLayout;
                return true;
            }
            catch
            {
                return false;
            }
        }
        private RectangleF GetImageLayOut()
        {
            return this.twain.ImageLayout;
        }
        private void AcquireCompleted()
        {
            //logger.Debug("AcquireCompleted()");
            if (twain.ImageCount > 0)
            {
                var img = twain.GetImage(0);

                var eventArg = new ImageChangedEventArgs(img);
                eventArg.IsScanLayoutSet = IsLayoutSet;

                ImageChanged(this, eventArg);
                Completed(this, new HWCompletedEventArgsEventArgs(""));
            }
            else
            {

                Error(this, new HWErrorEventArgs("Ошибка сканирования"));
            }
            //logger.Debug("AcquireCompleted() end");
        }

        private bool SetResolutions(int dpi)
        {
            var resolutions = twain.Capabilities.XResolution.Get();

            var resList = new List<int>();
            var resListo = new List<object>();

            for (var i = 0; i < resolutions.Count; i++)
            {
                resList.Add(Convert.ToInt32(resolutions[i]));
                resListo.Add(resolutions[i]);
            }

            if (resList.Contains(dpi))
            {
                twain.Capabilities.XResolution.Set((float)dpi);
                twain.Capabilities.YResolution.Set((float)dpi);
                return true;
            }

            if (resolutions.Count > 0)
            {

                twain.Capabilities.XResolution.Set((float)resolutions[resolutions.Count - 1]);
                twain.Capabilities.YResolution.Set((float)resolutions[resolutions.Count - 1]);
                return true;
            }
            else
            {
                Error(this, new HWErrorEventArgs("Не могу установить разрешение для сканера"));
                return false;
            }
            // return true;
        }

        private void setPixelType()
        {
            //var pixels = twain.Capabilities.PixelType.Get();

            //var pixelType = TwPixelType.Gray;
            //for (int i = 0; i < pixels.Count; i++)
            //{
            //    if (((TwPixelType)pixels[i]) == TwPixelType.Gray)
            //    {
            //        pixelType = TwPixelType.Gray;
            //        break;
            //    }
            //    if (((TwPixelType)pixels[i]) == TwPixelType.BW)
            //    {
            //        pixelType = TwPixelType.BW;
            //        break;
            //    }
            //}

            //twain.Capabilities.PixelType.Set(pixelType);

        }


        #region even handlers

        private void endXferEventHandler(object sender, Twain32.EndXferEventArgs eventsArgs)
        {
            //logger.Debug("endXferEventHandler");
            try
            {
                var eventArg = new ImageChangedEventArgs((Image)eventsArgs.Image.Clone());
                eventArg.IsScanLayoutSet = IsLayoutSet;

                ImageChanged(this, eventArg);
                eventsArgs.Image.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //  TODO: add logger
                Error(this, new HWErrorEventArgs("Ошибка сканирования"));
            }

            //logger.Info("endXferEventHandler end");
        }

        private void acquireCompletedEventHandler(object sender, EventArgs eventsArgs)
        {
            logger.Info("acquireCompletedEventHandler");
            twain.CloseDataSource();
            twain.CloseDSM();
            Completed(this, new HWCompletedEventArgsEventArgs(""));
            logger.Info("acquireCompletedEventHandler end");
        }

        private void acquireErrorEventHandler(object sender, Twain32.AcquireErrorEventArgs eventsArgs)
        {
            logger.Info("acquireErrorEventHandler end");

            twain.CloseDataSource();
            twain.CloseDSM();
            if (eventsArgs.Exception.ReturnCode == TwRC.Cancel)
            {
                AcquireCompleted();
            }
            else
            {
                var msg = string.Format("Ошибка сканирования: ReturnCode = {0}; ConditionCode = {1};", eventsArgs.Exception.ReturnCode, eventsArgs.Exception.ConditionCode);
                Error(this, new HWErrorEventArgs(msg));
            }

            var inner = eventsArgs.Exception.InnerException == null ? "" : eventsArgs.Exception.InnerException.Message + eventsArgs.Exception.InnerException.StackTrace;
            logger.Info(string.Format("ReturnCode: {0}, Error: {1}, InnerError: {2}", eventsArgs.Exception.ReturnCode, eventsArgs.Exception.Message + Environment.NewLine + eventsArgs.Exception.StackTrace, inner));

            logger.Info("acquireErrorEventHandler end");

        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (twain != null)
                {
                    twain.CloseDataSource();
                    twain.CloseDSM();
                    twain.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        #endregion



        private IntPtr parentWindowHandle;
        public IntPtr ParentWindowHandle 
        {
            get{return parentWindowHandle ;}
            set
            {
                parentWindowHandle = value;
                if(parentWindowHandle == IntPtr.Zero)
                    return;

                var parentWnd = new NativeWindow();
                parentWnd.AssignHandle(this.ParentWindowHandle);

                if (twain != null)
                {
                    
                    twain.Parent = parentWnd;
                }
            }
        }
    }
}
