using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Topshelf;
using System.Windows.Forms;
using PostUserActivity.Forms;

namespace PostUserActivity.Service
{

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern Boolean AllocConsole();
    }


    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //http://stackoverflow.com/questions/1627014/windows-form-from-console

            Application.EnableVisualStyles();
            //Application.Run(new ScannersSettings());
            //if (args != null && args.Count() > 0)
            //{

            //}
            //Application.DoEvents();

            HostFactory.Run(x =>                                 //1            
            {
                x.Service<LoaderService>(s =>                        //2
                {
                    s.ConstructUsing(name => new LoaderService());     //3
                    s.WhenStarted(tc =>
                    {                       
                        tc.Start();                     
                    });              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                    s.BeforeStoppingService(tc =>
                    {
                    
                    });
                    s.AfterStoppingService(tc =>
                    {
                      
                    });
                });
                x.RunAsLocalSystem();                            //6
                //x.RunAsNetworkService();    
                x.SetDescription("PostARM.Service");        //7
                x.SetDisplayName("PostARM.Service");                       //8
                x.SetServiceName("PostARM.Service");                       //9
                x.StartAutomatically();

                
                //x.UseNLog();

                x.EnablePauseAndContinue();
                x.EnableShutdown();
            });

            
            while (!true)
            {
                Application.DoEvents(); //Now if you call "form.Show()" your form won´t be frozen
                                        //Do your stuff
            }
        }
    }
}
