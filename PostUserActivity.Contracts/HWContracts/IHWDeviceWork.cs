using System;
using System.Security.Cryptography.X509Certificates;

namespace PostUserActivity.Contracts.HWContracts
{
    /// <summary>
    /// общий интерфейс описывающий работу с устройствами (запуск, остановка, ошибки)
    /// </summary>
    public interface IHWDeviceWork : IDisposable
    {
        void Start();
        void Stop();

        void StartRecord();

        bool IsRunning();
        event EventHandler<HWCompletedEventArgsEventArgs> Completed;
        event EventHandler<HWErrorEventArgs> Error;
        event EventHandler<ImageChangedEventArgs> ImageChanged;

        event EventHandler<ImageChangedEventArgs> PreviewImageChanged;

        IntPtr ParentWindowHandle { get; set; }
    }    
}
