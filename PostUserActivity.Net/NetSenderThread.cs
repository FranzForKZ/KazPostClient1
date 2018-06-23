using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CommonLib;
using Newtonsoft.Json;
using NLog;
using PostUserActivity.Contracts.Network;
using Timer = System.Timers.Timer;

namespace PostUserActivity.Net
{
    public class NetSenderThread : INetSenderThread
    {
        #region fields & properties

        private Timer timer;

        private static bool sendingProcess = false;
        private static object lockObject = new object();

        private static Logger logger;

        private TimeSpan sendTimeStart;
        private TimeSpan sendTimeEnd;

        private TimeSpan sleepTimeOnError;
        private TimeSpan sleepTimeOnNoneFiles;
        private TimeSpan sleepTimeOnWaitSendOnTime;
        #endregion


        #region ctor

        public NetSenderThread()
        {
            logger = LogManager.GetCurrentClassLogger();
            //  срабатывает каждые 10 секунд
            
            
            timer = new Timer(10 * 1000);
            timer.Elapsed += Timer_Elapsed;
            
        }

        #endregion



        #region Implementation of INetSenderThread

        public void SenderTread()
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            var v1 = System.Configuration.ConfigurationManager.AppSettings["SendTimeStart"] ?? "00-00";
            var v2 = System.Configuration.ConfigurationManager.AppSettings["SendTimeEnd"] ?? "00-00";
            sendTimeStart = timeSpanfromString(v1);
            sendTimeEnd = timeSpanfromString(v2);

            var slError = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnErrorMin"]) ? "30" : System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnErrorMin"];
            var slErrorSec = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnErrorSec"]) ? "30" : System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnErrorSec"];
            sleepTimeOnError = new TimeSpan(0, int.Parse(slError), int.Parse(slErrorSec));

            var slOnNoneFiles = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnNoneFilesMin"]) ? "10" : System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnNoneFilesMin"];
            var slOnNoneFilesSec = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnNoneFilesSec"]) ? "10" : System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnNoneFilesSec"];

            sleepTimeOnNoneFiles = new TimeSpan(0, int.Parse(slOnNoneFiles), int.Parse(slOnNoneFilesSec));

            var slWaitSendOnTime = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnWaitSendOnTimeMin"]) ? "30" : System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnWaitSendOnTimeMin"];
            var slWaitSendOnTimeSec = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnWaitSendOnTimeSec"]) ? "30" : System.Configuration.ConfigurationManager.AppSettings["SlepTimeOnWaitSendOnTimeSec"];
            sleepTimeOnWaitSendOnTime = new TimeSpan(0, int.Parse(slWaitSendOnTime), int.Parse(slWaitSendOnTimeSec));

            timer.Start();
        }

        #endregion


        private Func<TimeSpan, TimeSpan, bool> isBetween = (t1, t2) =>
        {
            var now = DateTime.Now.TimeOfDay;

            if (t1 > t2)
            {
                return (t1 < now) || (now < t2);
            }
            else
            {
                return t1 < now && now < t2;
            }
        };

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            
            if (sendTimeStart != sendTimeEnd)
            {
                if (!isBetween(sendTimeStart, sendTimeEnd))
                {
                    logger.Info("It is not the time to start sending. Start time: {0}, end time: {1}, now: {2}. Sleep: {3}", sendTimeStart, sendTimeEnd, DateTime.Now, sleepTimeOnWaitSendOnTime);
                    Thread.Sleep(sleepTimeOnWaitSendOnTime);
                    return;
                }
            }

            if (!sendingProcess)
            {
                lock (lockObject)
                {
                    if (!sendingProcess)
                    {
                        sendingProcess = true;

                        var config = new CommonLib.ConfigurationSettings();
                        var filesPath = config.GetUploadPath();
                        var files = Directory.GetFiles(filesPath, "*.json");

                        if (files != null && files.Length > 0)
                        {
                            var fileToSend = getOlderFile(files);
                            var req = new NetRequests();
                            req.updateUrls();
                            string fileContent = "";
                            if (FileHelper.ReadFile(fileToSend, out fileContent))
                            {
                                ArmDataPackage package = null;
                                try
                                {
                                    package = JsonConvert.DeserializeObject<ArmDataPackage>(fileContent);
                                }
                                catch(Exception ex)
                                {
                                    //  если файл попался хреновый, то мы его удалим, но и запишем его содержимое в лог ошибок
                                    logger.Error(ex, string.Format("Exception when read file packet json. \r\n Packet Content: {0}", fileContent));
                                    if(File.Exists(fileToSend))
                                    File.Delete(fileToSend);
                                }
                                
                                if (package == null)
                                    return;
                                var sendResult = req.SendPackage(package);
                                if (sendResult.RequestResult == RequestResultType.Successful)
                                {
                                    //  del file
                                    logger.Info("Package {0} sending succesfuly", fileToSend);
                                    if (File.Exists(fileToSend))
                                    File.Delete(fileToSend);
                                    logger.Info("Package {0} deleted", fileToSend);
                                }
                                else
                                {
                                    logger.Error("Coudn't send package {0}, Back-End return error: ({1}){2}. Sleep: {3}", fileToSend, (int)sendResult.RequestResult, sendResult.RequestResult, sleepTimeOnError);
                                    //  ошибка при отправке файла, спим
                                    Thread.Sleep(sleepTimeOnError);
                                }
                                
                            }
                        }
                        else
                        {
                            //  спим, если не нашли файлов для отправки
                            Thread.Sleep(sleepTimeOnNoneFiles);
                            logger.Info("There are no files to send. Sleep: {0}", sleepTimeOnNoneFiles);
                        }
                        sendingProcess = false;
                    }
                }
            }
        }


        private Func<string, TimeSpan> timeSpanfromString = (str) =>
        {
            var regex = new Regex(@"(?<HH>\d{1,2})-(?<mm>\d{1,2})");
            if (regex.IsMatch(str))
            {
                var match = regex.Match(str);
                return new TimeSpan(int.Parse(match.Groups["HH"].Value), int.Parse(match.Groups["mm"].Value), 0);
            }
            return new TimeSpan(0, 0, 0);
        };

        //private bool readFile(string fileName, out string fileContent)
        //{
        //    fileContent = "";
        //    try
        //    {
        //        using (Stream stream = new FileStream(fileName, FileMode.Open))
        //        using (var streamReader = new StreamReader(stream))
        //        {
        //            fileContent = streamReader.ReadToEnd();
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        //check here why it failed and ask user to retry if the file is in use.
        //        return false;
        //    }
        //}

        private Func<string[], string> getOlderFile = (files) =>
        {
                var fd = getFilesWithDates(files);
                var minDate = fd.Min(p => p.Key);
                return fd.First(p => p.Key == minDate).Value;
        };

        private static IEnumerable<KeyValuePair<DateTime, string>> getFilesWithDates(string[] files)
        {
            var regex = new Regex(@"packet_(?<dateTime>[\d\s-]+)_[\d]+_[\d]+_[\w]+.json");
            foreach (var fileName in files)
            {
                if (regex.IsMatch(fileName))
                {
                    var dtStr = regex.Match(fileName).Groups["dateTime"].Value;
                    var date = DateTime.ParseExact(dtStr, NetSender.fileDateTemplate, CultureInfo.InvariantCulture);
                    yield return new KeyValuePair<DateTime, string>(date, fileName);
                }
            }

        }
    }
}
