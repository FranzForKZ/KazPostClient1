using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CommonLib
{
    /// <summary>
    /// запуск winForms окна из под другого потока
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WinFormStarter<T> where T : Form
    {
        public void Show(params object[] args)
        {
            Thread t = new Thread(() => StartNewStaThreadParams(args[0], args[1]));
            t.Start();
        }

        public void Show()
        {
            Thread t = new Thread(new ThreadStart(StartNewStaThread));

            t.Start();
        }

        [STAThread]
        private void StartNewStaThreadParams(params object[] args)
        {
            Form form = (T)Activator.CreateInstance(typeof(T), args);
            Application.Run(form);
        }

        [STAThread]
        private void StartNewStaThread()
        {
            Form form = (T)Activator.CreateInstance(typeof(T));
            Application.Run(form);
        }
    }
}
