namespace TerraSketch.View
{
    partial class FrmAbout
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
            this.tbAboutInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbAboutInfo
            // 
            this.tbAboutInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAboutInfo.Location = new System.Drawing.Point(0, 0);
            this.tbAboutInfo.Multiline = true;
            this.tbAboutInfo.Name = "tbAboutInfo";
            this.tbAboutInfo.Size = new System.Drawing.Size(457, 358);
            this.tbAboutInfo.TabIndex = 0;
            this.tbAboutInfo.Text = "Nothing yet";
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 358);
            this.Controls.Add(this.tbAboutInfo);
            this.Name = "FrmAbout";
            this.Text = "FrmAbout";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbAboutInfo;
    }
}