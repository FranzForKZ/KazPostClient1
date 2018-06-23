using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Collections;
namespace AfterInstall
{
    class Program
    {
        static private string[] AllPathsAsStringArr(string Paths)
        {
            string[] temp = Paths.Split(new[] { ';' }, StringSplitOptions.None);
            return temp;
        }

        static string cutPythonFromPaths(string[] PathsArr)
        {
            var dat1 = DateTime.Now;
            StringBuilder ReturnVal = new StringBuilder();
            foreach (var item in PathsArr)
            {
                if (item.IndexOf("Python27") == -1)
                {
                    ReturnVal.Append(item + ';');
                }
            }
            var dat2 = DateTime.Now;
            return ReturnVal.ToString();
        }
        static string MakePathWithAltDirectorySeparator(string Item)
        {
            string retVal = "";
            for (int i = 0; i < Item.Count(); i++)
            {
                if(Item[i] == Path.DirectorySeparatorChar)  // '\'
                {
                    retVal+= Path.AltDirectorySeparatorChar; // '/'
                    continue;
                }
                retVal += Item[i];
            }
            return retVal;
        }
        static void Main(string[] args)
        {
            //using (EventLog eventLog = new EventLog("Application"))
            //{
            //    eventLog.Source = "Application";
            //    eventLog.WriteEntry("First element " + args[0], EventLogEntryType.Error, 101, 0);
            //}
            string appPath = "";
            appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (args.Count() >= 1)
            {
                //Python registration ---------------------------------------------------------------------
                var EnviromentVars = Environment.GetEnvironmentVariables();
                bool Python27Exist = false;
                foreach (DictionaryEntry item in EnviromentVars)
                {

                    if (item.Value.ToString().IndexOf("Python27") != -1)
                    {
                        Python27Exist = true;
                        break;
                    }
                }
                if (!Python27Exist)
                {
                    //reg python logic
                    var value = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
                    string Python27Paths = ";" + appPath + "\\Python27;" + appPath + "\\Python27\\Scripts";
                    Environment.SetEnvironmentVariable("Path", value + Python27Paths, EnvironmentVariableTarget.Machine);
                }
               // System.Threading.Thread.Sleep(10000);
                // -----------------------------------------------------------------------------------------------------------
                //Args[0] should be PythonSrc path
                //register pythonObject
                //string strCmdText = "";
                using (EventLog eventLog = new EventLog("Application"))
                {
                    string parametrs = " ";
                    foreach (var item in args)
                    {
                        parametrs += item + " \n";
                    } 
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Passed parametr: " + args[0], EventLogEntryType.Error, 560, 0);
                }
                string strCmdText = "/C " + appPath + "\\Python27\\" + "python.exe " + appPath + "\\PythonSrc\\frame_quality_analyser_com.py --register";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                //------------------------------------------------------------------------------------------------------------
                //Configuration edit -----------------------------------------------------------------------------------------
                string configFile = System.IO.Path.Combine(appPath, "KazPostARM.exe.config");
                
                //using (EventLog eventLog = new EventLog("Application"))
                //{
                //    string parametrs = " ";
                //    foreach (var item in args)
                //    {
                //        parametrs += item + " \n";
                //    }
                //    eventLog.Source = "Application";
                //    eventLog.WriteEntry("Config file path: " + configFile, EventLogEntryType.Error, 101, 0);
                //}
                if (File.Exists(configFile))
                {
                    //Open Config File
                    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                    configFileMap.ExeConfigFilename = @configFile;
                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                    // Change Python detector value
                    config.AppSettings.Settings["PythonDetectorPath"].Value = MakePathWithAltDirectorySeparator(args[0]);
                    config.Save();
                }
               // Console.ReadKey();
                //------------------------------------------------------------------------------------------------------------------------
            }
            else
            {
                //unregistration begin----------------------------------------------------------------------------------------------------
                //unregister python object
                string configFile = System.IO.Path.Combine(appPath, "KazPostARM.exe.config");
                if (File.Exists(configFile))
                {
                    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                    configFileMap.ExeConfigFilename = @configFile;
                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                    // Change Python detector value
                    string strCmdText = "/C " + appPath + "\\Python27\\" + "python.exe " + appPath + "\\PythonSrc\\frame_quality_analyser_com.py --unregister";
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "CMD.EXE";
                    psi.Arguments = strCmdText;

                   var process = System.Diagnostics.Process.Start(psi);
                   process.Close();
                   //process.WaitForExit();
                   //if (process.ExitCode == 0)
                   //{
                   //    Console.WriteLine("Unregistration completed successfuly");
                   //}
                   // Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Unregister Error. Should be at least 1 paramentr as string with PythonsSrc path to invoke registration logic");
                    Console.WriteLine("Make Sure you run this app in KazPostArm main folder");
                    //Console.ReadKey();
                }
                //-----------------------------------------------------------------------------------------------------------------------
                //unregister PATH ENV

                string value = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
                var PathsStringArr = AllPathsAsStringArr(value);
                var PathWithOutPython = cutPythonFromPaths(PathsStringArr);
                Environment.SetEnvironmentVariable("Path", PathWithOutPython, EnvironmentVariableTarget.Machine);
                var yamlConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostUserActivity");
                if(Directory.Exists(yamlConfig))
                {
                    string[] files = Directory.GetFiles(yamlConfig);
                    foreach (string file in files)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                    Directory.Delete(yamlConfig);
                    Console.WriteLine("File deleted");
                }
                else
                {
                    Console.WriteLine("File not deleted");
                }
                //Console.ReadKey();
               // Console.ReadKey();
                //string Python27Paths = ";" + appPath + "\\Python27;" + appPath + "\\Python27\\Scripts";
                //Environment.SetEnvironmentVariable("Path", value + Python27Paths, EnvironmentVariableTarget.Machine);

                //-----------------------------------------------------------------------------------------------------------------------
                //log into Event log-----------------------------------------------------------------------------------------------------
                //using (EventLog eventLog = new EventLog("Application"))
                //{
                //    string parametrs = " ";
                //    foreach (var item in args)
                //    {
                //        parametrs += item + " \n";
                //    }
                //    eventLog.Source = "Application";
                //    eventLog.WriteEntry("After install Program must have  2 parameter with PythonSrc path as string and register/unregister as string only \n Program Started with Parametrs: " + parametrs, EventLogEntryType.Error, 0, 0);
                //}
                //-------------------------------------------------------------------------------------------------------------------------
            }
        }
    }
}
