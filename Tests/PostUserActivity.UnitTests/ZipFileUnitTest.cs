using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonLib;
using KazPostARM;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using PostUserActivity.Net;
using PostUserActivity.Contracts.Network;
namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class ZipFileUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {


            //string Path = "";
            var TimeStamp = DateTime.Now;
            List<string> FilesToSend = null;
            List<string> JpgFilesToDelete = null;
            SetJpgFileLists(out FilesToSend, out JpgFilesToDelete);

            var tempFolder = Path.Combine(Path.GetTempPath(), "ArchivesForPackages");
            Directory.CreateDirectory(tempFolder);
            foreach (var item in FilesToSend)
            {
                if (File.Exists(item))
                    FileHelper.CreateZip(item, Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(item) + ".zip"));
              
            }
            DirectoryInfo DI = new DirectoryInfo(tempFolder);
            var ZipCollection = DI.GetFiles("*.zip");
            List<LogPackage> packets = new List<LogPackage>();
            foreach (var item in ZipCollection)
            {
                var paket = CreateLogPacket(item.Name, item.FullName, TimeStamp);
                //paket.FileName = "some string";
                packets.Add(paket);
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(paket);
                }
                catch (Exception ex)
                {

                }
                
            }

            //ChangeWebCamFullImage(ii.FullImage);
            var config = new CommonLib.ConfigurationSettings();

            var netSender = new NetSender(config);

            var sendedResult = netSender.Send(packets);

        }

        private void SetJpgFileLists(out List<string> NewFilesToSend, out List<string> OldFilesToDelete)
        {
            NewFilesToSend = new List<string>();
            OldFilesToDelete = new List<string>();

            NewFilesToSend.Add("KazPostARM.exe.config");
            NewFilesToSend.Add("//logs//info.log");
            NewFilesToSend.Add("//logs//debug.log");
            NewFilesToSend.Add("//logs//exception.log");
            //DirectoryInfo DI = new DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            //var FilesInfo = DI.GetFiles("*.jpg").ToList();
            //DateTime NewiestFile = new DateTime();
            //if (FilesInfo.Count > 0)
            //{
            //    NewiestFile = FilesInfo[FilesInfo.Count - 1].CreationTime;
            //    TimeSpan TimeCreationDifference = new TimeSpan();
            //    int FileLifeDuration;
            //    int.TryParse(ConfigurationManager.AppSettings["AnalyseTimeInSec"], out FileLifeDuration);
            //    for (int i = 0; i < FilesInfo.Count - 1; i++)
            //    {
            //        TimeCreationDifference = NewiestFile - FilesInfo[i].CreationTime;

            //        if (TimeCreationDifference.TotalSeconds > FileLifeDuration)
            //        {
            //            OldFilesToDelete.Add(FilesInfo[i].FullName);
            //        }
            //        else
            //        {
            //            NewFilesToSend.Add(FilesInfo[i].FullName);

            //        }
            //    }
            //    //Add to list newestOne
            //    NewFilesToSend.Add(FilesInfo[FilesInfo.Count - 1].FullName);
            //}
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
                WorkStation = Environment.MachineName,
                TimeStamp = timestamp,
                Type = ArmDataPackageType.Log,
                FileName = LogName,
                FileContent = ArchiveToBase64(Path)
            };
            return LogPacket;
        }

        public string ArchiveToBase64(string Path)
        {
            try
            {
                var FileContent = File.ReadAllText(Path,System.Text.Encoding.UTF8);
                return StringExtensions.ToBase64(FileContent);
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }
    }
}
