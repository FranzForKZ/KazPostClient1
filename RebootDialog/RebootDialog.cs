using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
namespace RebootDialog
{
    public partial class RebootDialog : Form
    {
        //with api
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //internal struct TokPriv1Luid { public int Count; public long Luid; public int Attr;}
        //[DllImport("kernel32.dll", ExactSpelling = true)]
        //internal static extern IntPtr GetCurrentProcess();
        //[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        //internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        //[DllImport("advapi32.dll", SetLastError = true)]
        //internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        //[DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        //internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        //[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        //internal static extern bool ExitWindowsEx(int flg, int rea);
        //internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        //internal const int TOKEN_QUERY = 0x00000008;
        //internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        //internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        //internal const int EWX_LOGOFF = 0x00000000;
        //internal const int EWX_SHUTDOWN = 0x00000001;
        //internal const int EWX_REBOOT = 0x00000002;
        //internal const int EWX_FORCE = 0x00000004;
        //internal const int EWX_POWEROFF = 0x00000008;
        //internal const int EWX_FORCEIFHUNG = 0x00000010;
        //private void DoExitWin(int flg)
        //{
        //    bool ok;
        //    TokPriv1Luid tp;
        //    IntPtr hproc = GetCurrentProcess();
        //    IntPtr htok = IntPtr.Zero;
        //    ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
        //    tp.Count = 1;
        //    tp.Luid = 0;
        //    tp.Attr = SE_PRIVILEGE_ENABLED;
        //    ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
        //    ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        //    ok = ExitWindowsEx(flg, 0);
        //}
        //public void RestartComputerApi()
        //{
        //    Thread.Sleep(4000);
        //    DoExitWin(EWX_REBOOT);
        //}
        PostUserActivity.Net.NetRequests NetReq = new PostUserActivity.Net.NetRequests();
        PostUserActivity.Contracts.Network.ResponseResult ResResult;
        bool isDefind = false;
        public RebootDialog()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.TopLevel = true;
            //ResResult = NetReq.RegisterArm(PostUserActivity.HW.SystemInfo.GetUUID(), PostUserActivity.HW.SystemInfo.GetCurrentUserDomainName());
            //MessageBox.Show("код ответа сервера: " + (int)ResResult.RequestResult);
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
          //string gg = CommonLib.AppInfo.Version;

            Application.Exit();

        }

        private void RebootDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox.Checked)
            {
                try
                {
                    var psi = new ProcessStartInfo("shutdown", "/r /t 30");
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Произошла ошибка:" + ex.Message + "\n" + ex.StackTrace + "\n" + ex.Source + "\n" + ex.InnerException);
                }
                
            }
        }

        private void RebootDialog_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000;
            timer.Tick += (s, Z) =>
            {
                ((System.Windows.Forms.Timer)s).Stop(); //s is the Timer
                checkBox.Checked = false;
                Application.Exit();
            };
            timer.Start();
            
        }

       
    }
}
