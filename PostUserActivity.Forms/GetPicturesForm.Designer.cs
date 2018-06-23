namespace PostUserActivity.Forms
{
    partial class GetPicturesForm
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
            this.GetImage = new System.Windows.Forms.Button();
            this.Picture = new System.Windows.Forms.PictureBox();
            this.CloseForm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.SuspendLayout();
            // 
            // GetImage
            // 
            this.GetImage.Location = new System.Drawing.Point(306, 348);
            this.GetImage.Name = "GetImage";
            this.GetImage.Size = new System.Drawing.Size(157, 23);
            this.GetImage.TabIndex = 0;
            this.GetImage.Text = "Получить изображение";
            this.GetImage.UseVisualStyleBackColor = true;
            this.GetImage.Click += new System.EventHandler(this.GetImage_Click);
            // 
            // Picture
            // 
            this.Picture.Location = new System.Drawing.Point(12, 12);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(288, 388);
            this.Picture.TabIndex = 1;
            this.Picture.TabStop = false;
            // 
            // CloseForm
            // 
            this.CloseForm.Location = new System.Drawing.Point(306, 377);
            this.CloseForm.Name = "CloseForm";
            this.CloseForm.Size = new System.Drawing.Size(157, 23);
            this.CloseForm.TabIndex = 2;
            this.CloseForm.Text = "Закрыть";
            this.CloseForm.UseVisualStyleBackColor = true;
            this.CloseForm.Click += new System.EventHandler(this.CloseForm_Click);
            // 
            // GetPicturesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 412);
            this.Controls.Add(this.CloseForm);
            this.Controls.Add(this.Picture);
            this.Controls.Add(this.GetImage);
            this.MaximizeBox = false;
            this.Name = "GetPicturesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Изображение";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GetPicturesForm_FormClosing);
            this.Load += new System.EventHandler(this.GetPicturesForm_Load);
            this.Shown += new System.EventHandler(this.GetPicturesForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button GetImage;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.Button CloseForm;
    }
}