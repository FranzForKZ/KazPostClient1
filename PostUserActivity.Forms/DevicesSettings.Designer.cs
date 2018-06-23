namespace PostUserActivity.Forms
{
    partial class DevicesSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DevicesList = new System.Windows.Forms.ListBox();
            this.SaveSetting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DevicesList
            // 
            this.DevicesList.FormattingEnabled = true;
            this.DevicesList.Location = new System.Drawing.Point(12, 12);
            this.DevicesList.Name = "DevicesList";
            this.DevicesList.Size = new System.Drawing.Size(270, 238);
            this.DevicesList.TabIndex = 0;
            // 
            // SaveSetting
            // 
            this.SaveSetting.Location = new System.Drawing.Point(297, 226);
            this.SaveSetting.Name = "SaveSetting";
            this.SaveSetting.Size = new System.Drawing.Size(75, 23);
            this.SaveSetting.TabIndex = 1;
            this.SaveSetting.Text = "Сохранить";
            this.SaveSetting.UseVisualStyleBackColor = true;
            this.SaveSetting.Click += new System.EventHandler(this.SaveSetting_Click);
            // 
            // DevicesSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.SaveSetting);
            this.Controls.Add(this.DevicesList);
            this.MaximizeBox = false;
            this.Name = "DevicesSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            this.Load += new System.EventHandler(this.DevicesSettings_Load);
            this.Shown += new System.EventHandler(this.DevicesSettings_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox DevicesList;
        private System.Windows.Forms.Button SaveSetting;
    }
}