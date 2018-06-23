using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PostUserActivity.HW;
using CommonLib;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;


namespace PostUserActivity.Forms
{
    public partial class DevicesSettings : Form
    {
        private IHWSearcher devicesSearcher;
        private List<HWDeviceDesciption> devices;
        IDeviceConfiguration _deviceConfiguration;
        private HWDeviceDesciption savedDevice;
        private ISettingsDialog settings;
        
        private DevicesSettings()
        {
            InitializeComponent();
        }
        //ISettingsDialog
        public DevicesSettings(ISettingsDialog settings) : this()
        {
            _deviceConfiguration = settings.GetDeviceConfiguration();
            this.settings = settings;
            this.Text = settings.GetWindowHeaderText();
        }


        private void DevicesSettings_Load(object sender, EventArgs e)
        {
            
            devicesSearcher = new DeviceSearchFactory(settings.GetDeviceType());
            devices = devicesSearcher.GetDevices().ToList();

            savedDevice = _deviceConfiguration.GetDevice(settings.GetDeviceType());
            if (!devices.Any())
            {
                SaveSetting.Text = "Закрыть";
            }
        }

        private void SaveSetting_Click(object sender, EventArgs e)
        {
            
            if (devices.Any())
            {
                var selectedDevice = devices.First(p => p.Name == DevicesList.SelectedItem.ToString());
                _deviceConfiguration.SaveDevice(selectedDevice);
            }

            this.Close();
        }

        private void DevicesSettings_Shown(object sender, EventArgs e)
        {
            if (devices.Any())
            {
                DevicesList.DataSource = devices.Select(p => p.Name).ToList();
            }
            else
            {
                MessageBox.Show(settings.GetNotFoundMessage());
            }

            if (devices.Exists(p => p.Equals(savedDevice)))
            {
                DevicesList.SelectedItem = savedDevice.Name;
            }
        }
    }
}
