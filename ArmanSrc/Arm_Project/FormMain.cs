using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.Util;
using Emgu.CV.Util;


using Emgu.CV;                  //
using Emgu.CV.CvEnum;           // usual Emgu Cv imports
using Emgu.CV.Structure;        //
using Emgu.CV.UI;               //

namespace Arm_Project
{
    public partial class FormMain : Form
    {
        Capture _capWebcam =null;
        private bool _isStart=true;
        private VideoWriter _videoWriter = null;
        public FormMain()
        {
            InitializeComponent();
            _isStart = true;
            
        }
        #region Camera
        private void processFrame(object sender, EventArgs arg)
        {
            Mat imageFream = _capWebcam.QueryFrame();
            ibOriginal.Image = imageFream;
            _videoWriter.Write(imageFream);
            Thread.Sleep(0);

        }
        private void ReleaseData()
        {
            if (_capWebcam != null)
            {
                _capWebcam.Dispose();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (_isStart)
            {
                button1.Text = "Stop";
                try
                {
                    var name = Guid.NewGuid();
                    _capWebcam = new Capture(0);
                    _capWebcam.SetCaptureProperty(CapProp.FrameWidth, 1920);  
                    _capWebcam.SetCaptureProperty(CapProp.FrameHeight, 1080); 
                    _videoWriter = new VideoWriter(
                        name.ToString()+".avi",
                        -1,
                         new Size(1920,1080), 
                        true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("unable to read from webcam, error: " + Environment.NewLine + Environment.NewLine +
                                    ex.Message + Environment.NewLine + Environment.NewLine +
                                    "exiting program");
                    Environment.Exit(0);
                    return;
                }
                Application.Idle += processFrame;
            }
            else
            {
                Application.Idle -= processFrame;
                
                _capWebcam.Dispose();
                button1.Text = "Start";
            }
            _isStart = !_isStart;
        }
#endregion
        private void btnScan_Click(object sender, EventArgs e)
        {

        }
    }
}
