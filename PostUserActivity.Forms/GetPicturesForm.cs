using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonLib;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;
using PostUserActivity.HW;

namespace PostUserActivity.Forms
{
    public partial class GetPicturesForm : Form
    {
        private GetPicturesForm()
        {
            InitializeComponent();

        }

        private IHWSearcher hwSearcher;
        private IDeviceConfiguration config;
        //private DeviceType device;
        private HWDeviceDesciption savedDevice;

        private IHWDeviceWork deviceWorker;

        private DeviceWorkFactory devicesFactory;

        private IPictureDialog pictureDialog;

        private bool Successful = false;

        private int RetryCounter = 0;

        public GetPicturesForm(IPictureDialog pictureDialog) : this()
        {
            hwSearcher = new DeviceSearchFactory(pictureDialog.GetDeviceType());
            devicesFactory = new DeviceWorkFactory(pictureDialog.GetDeviceConfiguration());
            config = pictureDialog.GetDeviceConfiguration();
            this.Text = pictureDialog.GetHeaderText();
            this.GetImage.Text = pictureDialog.GetButtonText(RetryCounter);
            this.pictureDialog = pictureDialog;
        }

        public FormWorkResult GetFormWorkResult()
        {
            return new FormWorkResult()
            {
                IsSuccessful = Successful,
                FolderName = ((HWBase)deviceWorker).GetImagesFolderName()
            };
        }

        private void GetPicturesForm_Load(object sender, EventArgs e)
        {
            savedDevice = config.GetDevice(pictureDialog.GetDeviceType());
            deviceWorker = devicesFactory.GetDeviceWorkFactory(pictureDialog.GetDeviceType());

            deviceWorker.Completed += DeviceWorker_Completed;
            deviceWorker.Error += DeviceWorker_Error;
            deviceWorker.ImageChanged += DeviceWorker_ImageChanged;

        }

        private void DeviceWorker_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            this.Picture.Image = ImageResize.Resize(e.Img, this.Picture.Width, this.Picture.Height);
        }

        private void DeviceWorker_Error(object sender, HWErrorEventArgs e)
        {
            EnableButtons();
            MessageBox.Show(e.Message);
        }

        private void DeviceWorker_Completed(object sender, HWCompletedEventArgsEventArgs e)
        {
            Successful = true;
            var images = e.Images;
            EnableButtons();
        }


        private void EnableButtons()
        {
            if (CloseForm.InvokeRequired)
            {
                CloseForm.Invoke(new MethodInvoker(delegate { CloseForm.Enabled = true; }));
            }
            else { CloseForm.Enabled = true; }

            if (GetImage.InvokeRequired)
            {
                GetImage.Invoke(new MethodInvoker(delegate { GetImage.Text = pictureDialog.GetButtonText(RetryCounter); }));
                GetImage.Invoke(new MethodInvoker(delegate { GetImage.Enabled = true; }));
            }
            else
            {
                GetImage.Enabled = true;
                GetImage.Text = pictureDialog.GetButtonText(RetryCounter);
            }
        }

        private void GetImage_Click(object sender, EventArgs e)
        {
            RetryCounter++;
            Successful = false;
            deviceWorker.Start();
            this.CloseForm.Enabled = false;
            this.GetImage.Enabled = false;
        }

        private void GetPicturesForm_Shown(object sender, EventArgs e)
        {
            //  проверка, что у нас в системе установлено сохраненное в конфиге оборудование
            var devices = hwSearcher.GetDevices().ToList();

            if (!devices.Exists(p => p.Equals(savedDevice)))
            {
                if (string.IsNullOrEmpty(savedDevice.Name))
                {

                    MessageBox.Show(this.pictureDialog.GetEmptyConfigErrorText());
                }
                else
                {
                    MessageBox.Show(string.Format("Проверьте настройки подключения устройства {0}. {0} не найдено", savedDevice.Name));
                }

                //GetImage.Text = "Закрыть";
                GetImage.Enabled = false;
            }
        }

        private void GetPicturesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (deviceWorker != null)
            {
                deviceWorker.Dispose();
            }
        }

        private void CloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
