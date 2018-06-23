using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonLib;
using NLog;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.Network;

namespace PostUserActivity.Net
{
    public class NetSender 
    {
        private ConfigurationSettings configuration;

        private Logger logger;

      
        //  public const string logFileNameTemplate = "Logpacket_{0}_{1}_{2}_{3}.json";
        public const string fileNameTemplate = "packet_{0}_{1}_{2}_{3}.json";
        public const string fileDateTemplate = "yyyy-MM-dd HH-mm-ss-ffff";
       // public const string LogfileDateTemplate = "yyyy-MM-dd HH-mm-ss-ffff";

        public NetSender(IDeviceConfiguration configuration)
        {
            logger = LogManager.GetCurrentClassLogger();
            this.configuration = (ConfigurationSettings) configuration;

        }

        public TResponseResult<bool> Send(List<ArmDataPackage> packets)
        {
            RequestResultType requestResult = RequestResultType.ConnectionUnavailable;
            var isSended = false;
            //  пытаемся отправить серверу, если нет - то сохраняем сообщения в папке для отправки
            var req = new NetRequests();
            req.updateUrls();
            foreach (var packet in packets)
            {
                var result = req.SendPackage(packet);
                var jsonFileContent = Newtonsoft.Json.JsonConvert.SerializeObject(packet);
                requestResult = result.RequestResult;
                logger.Info("Sendind paket result: ({0}){1}{2}Paket Content: {3}", (int)result.RequestResult, result.RequestResult, Environment.NewLine, jsonFileContent);
                if (result.RequestResult != RequestResultType.Successful)
                {
                    //  save in folder
                    logger.Error("Sendind paket exception: ({0}){1}{2}Paket Content: {3}", (int)result.RequestResult, result.RequestResult, Environment.NewLine, jsonFileContent);
                    var jsonFileName = string.Format(fileNameTemplate, packet.Timestamp.ToString(fileDateTemplate), packet.WFMId, packet.IIN, packet.Type);
                    
                    using (var writer = new System.IO.StreamWriter(Path.Combine(configuration.GetUploadPath(), jsonFileName)))
                    {
                        writer.Write(jsonFileContent);
                    }
                }
                else
                {
                    isSended = true;
                }

            }
             return new TResponseResult<bool>(isSended, requestResult);
        }
             public TResponseResult<bool> Send(List<LogPackage> packets)
             {
                RequestResultType requestResult = RequestResultType.ConnectionUnavailable;
                var isSended = false;
                //  пытаемся отправить серверу, если нет - то сохраняем сообщения в папке для отправки
                var req = new NetRequests();
                foreach (var packet in packets)
                {
                    //SendToDisk !!! Request will not execute. It will always return error, then save on disk
                    var result = req.SendLogPackage(packet);
                    var jsonFileContent = Newtonsoft.Json.JsonConvert.SerializeObject(packet);
                    requestResult = result.RequestResult;
                   // logger.Info("Sendind paket result: ({0}){1}{2}Paket Content: {3}", (int)result.RequestResult, result.RequestResult, Environment.NewLine, jsonFileContent);
                    if (result.RequestResult != RequestResultType.Successful)
                    {
                        //  save in folder
                        //logger.Error("Sendind paket exception: ({0}){1}{2}Paket Content: {3}", (int)result.RequestResult, result.RequestResult, Environment.NewLine, jsonFileContent);


                        var jsonFileName = string.Format(fileNameTemplate, DateTime.Now.ToString(fileDateTemplate), packet.WFMId, packet.IIN, packet.Type);
                    
                        using (var writer = new System.IO.StreamWriter(Path.Combine(configuration.GetUploadPath(), jsonFileName)))
                        {
                            writer.Write(jsonFileContent);
                        }
                    }
                    else
                    {
                        isSended = true;
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
                      return new TResponseResult<bool>(isSended, requestResult);
             }
  
    }
}
