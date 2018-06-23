using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.HWContracts;
using Saraff.Twain;
using NLog;

namespace PostUserActivity.HW
{
    /// <summary>
    /// поиск установленных в системе сканеров
    /// </summary>
    internal class ScannersSearcher : IHWSearcher
    {
        Logger logger;

        public ScannersSearcher()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }
               

        #region Implementation of IHWSearcher

        public IEnumerable<HWDeviceDesciption> GetDevices()
        {
            var twain = new Twain32();
            var isDsmOpened = false;

            try
            {
                //twain.OpenDataSource();
                isDsmOpened = twain.OpenDSM();
                if (twain.IsTwain2Supported)
                {
                    twain.IsTwain2Enable = true;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            
            if (isDsmOpened)
            {
                for (int i = 0; i < twain.SourcesCount; i++)
                {
                    yield return new HWDeviceDesciption
                    {
                        Device = DeviceType.Scanner,
                        Name = twain.GetSourceProductName(i),
                        DeviceId = i.ToString()
                    };
                }
            }

            if (twain != null)
            {
                twain.CloseDataSource();
                twain.CloseDSM();
                twain.Dispose();
            }

            yield break;
        }

        #endregion
    }
}
