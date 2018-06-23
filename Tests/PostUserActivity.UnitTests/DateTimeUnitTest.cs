using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class DateTimeUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var dt = DateTimeOffset.Now;
            string sign = dt.Offset < TimeSpan.Zero ? "-" : "+";
            int TotalMinutes = dt.Offset.Hours * 60 + dt.Offset.Minutes;

            int minutesFromConfig = 180;
            TimeSpan interval = new TimeSpan(0, minutesFromConfig, 0);

            dt = dt - interval;

            Console.WriteLine(dt.ToString("yyyyMMddHHmmss.ffffff") + sign + TotalMinutes.ToString());
        }
        [TestMethod]
        public void TestMethod2()
        {
            string UrlToDevide = "http://172.30.73.54:9080/client/rest/fullframe";
            //"http://172.30.73.54:9080/client/rest/fullframe"/>
            var index = UrlToDevide.IndexOf(":",6);
            var devideIndex  = 0;
            for (int i = index; i < UrlToDevide.Length; i++)
            {
                if (char.IsLetter(UrlToDevide[i]))
                {
                    devideIndex = i - 1;
                    break;
                }
                    

            }
            Console.WriteLine(UrlToDevide.Substring(devideIndex));
            Console.WriteLine(UrlToDevide.Substring(0,devideIndex ));

        }
        [TestMethod]
        public void TestMethod3()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), "ArchivesForPackages");
            Directory.CreateDirectory(tempFolder);
            var DI = new DirectoryInfo(tempFolder);
            var htmlCollection = DI.GetFiles("*.html");
            foreach (var item in htmlCollection)
            {
                //get all <td> that need to change
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                string htmlString = File.ReadAllText(item.FullName);
                document.LoadHtml(htmlString);
                string XPathExpression = ".//td";
                HtmlAgilityPack.HtmlNodeCollection collection = document.DocumentNode.SelectNodes(XPathExpression);
                foreach (var link in collection)
                {
                    string target = link.InnerText;
                }


                //edit timestamp  to human readable
            }
        }
        
    }
}
