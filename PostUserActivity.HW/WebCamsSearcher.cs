using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Video.DirectShow;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.HW
{
    /// <summary>
    /// поиск установленных в системе камер
    /// </summary>
    internal class WebCamsSearcher : IHWSearcher
    {
        #region Implementation of IHWSearcher

        public IEnumerable<HWDeviceDesciption> GetDevices()
        {
            //  Emgu.CV
            //  http://www.emgu.com/wiki/index.php/Camera_Capture
            //  http://www.codeproject.com/Articles/722569/Video-Capture-using-OpenCV-with-Csharp
            //  http://www.emgu.com/forum/viewtopic.php?t=3095
            //DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < videoDevices.Count; i++)
            {
                yield return new HWDeviceDesciption
                {
                    Device = DeviceType.WebCam,
                    Name = videoDevices[i].Name,
                    DeviceId = videoDevices[i].MonikerString
                };
            }
        }

        #endregion
    }
}
