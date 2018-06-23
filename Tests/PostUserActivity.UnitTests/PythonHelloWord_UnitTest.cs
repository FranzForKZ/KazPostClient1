using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.IO;
namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class PythonHelloWord_UnitTest
    {
        [TestMethod]
        public void CommunicateWithPython_TestMethod()
        {
         //   Type pythonServer;
         //   pythonServer = Type.GetTypeFromProgID("Python.QualityAnalyserCom");
         //   var pythonObject = Activator.CreateInstance(pythonServer);
         //   Debug.WriteLine(pythonObject.ToString());
         //   //
         //var  frame  =  new Bitmap(Image.FromFile("C:\\photo.jpg"));

         //   object result = pythonServer.InvokeMember("check_frame",
         //       BindingFlags.InvokeMethod, null, pythonObject, new object[] { frame });

         //   Debug.WriteLine(result.ToString());

         //   //MethodInfo methodInfo = pythonServer.GetMethod("Hello");
         //   //Console.WriteLine(methodInfo.ToString());
         //   //pythonObject.Hello();

         //  Debug.WriteLine("Completed");
            //Console.ReadKey();
            DirectoryInfo DirInf = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
           // DirInf.MoveTo("..\\..\\..\\");
            //DirInf.Parent.Parent.FullName
            string PathFolder1 = Path.GetDirectoryName(DirInf.Parent.Parent.Parent.FullName);
            string sd;
           //var uri = new System.Uri("../../../" +AppDomain.CurrentDomain.BaseDirectory);
           //var convertedPath = uri.AbsolutePath;


        }
    }
}
