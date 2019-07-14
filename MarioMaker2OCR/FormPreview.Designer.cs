namespace MarioMaker2OCR
{
    partial class FormPreview
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPreview));
            this.tabsPreviewer = new System.Windows.Forms.TabControl();
            this.tabLivePreview = new System.Windows.Forms.TabPage();
            this.imgLiveFrame = new Emgu.CV.UI.ImageBox();
            this.tabLastMatch = new System.Windows.Forms.TabPage();
            this.imgLastMatch = new Emgu.CV.UI.ImageBox();
            this.tabsPreviewer.SuspendLayout();
            this.tabLivePreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgLiveFrame)).BeginInit();
            this.tabLastMatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgLastMatch)).BeginInit();
            this.SuspendLayout();
            // 
            // tabsPreviewer
            // 
            this.tabsPreviewer.Controls.Add(this.tabLivePreview);
            this.tabsPreviewer.Controls.Add(this.tabLastMatch);
            this.tabsPreviewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsPreviewer.Location = new System.Drawing.Point(0, 0);
            this.tabsPreviewer.Name = "tabsPreviewer";
            this.tabsPreviewer.SelectedIndex = 0;
            this.tabsPreviewer.Size = new System.Drawing.Size(800, 450);
            this.tabsPreviewer.TabIndex = 0;
            // 
            // tabLivePreview
            // 
            this.tabLivePreview.Controls.Add(this.imgLiveFrame);
            this.tabLivePreview.Location = new System.Drawing.Point(4, 29);
            this.tabLivePreview.Name = "tabLivePreview";
            this.tabLivePreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabLivePreview.Size = new System.Drawing.Size(792, 417);
            this.tabLivePreview.TabIndex = 0;
            this.tabLivePreview.Text = "Live View";
            this.tabLivePreview.UseVisualStyleBackColor = true;
            // 
            // imgLiveFrame
            // 
            this.imgLiveFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgLiveFrame.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.imgLiveFrame.Location = new System.Drawing.Point(3, 3);
            this.imgLiveFrame.Name = "imgLiveFrame";
            this.imgLiveFrame.Size = new System.Drawing.Size(786, 411);
            this.imgLiveFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgLiveFrame.TabIndex = 2;
            this.imgLiveFrame.TabStop = false;
            // 
            // tabLastMatch
            // 
            this.tabLastMatch.Controls.Add(this.imgLastMatch);
            this.tabLastMatch.Location = new System.Drawing.Point(4, 29);
            this.tabLastMatch.Name = "tabLastMatch";
            this.tabLastMatch.Padding = new System.Windows.Forms.Padding(3);
            this.tabLastMatch.Size = new System.Drawing.Size(792, 417);
            this.tabLastMatch.TabIndex = 1;
            this.tabLastMatch.Text = "Last Match";
            this.tabLastMatch.UseVisualStyleBackColor = true;
            // 
            // imgLastMatch
            // 
            this.imgLastMatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgLastMatch.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.imgLastMatch.Location = new System.Drawing.Point(3, 3);
            this.imgLastMatch.Name = "imgLastMatch";
            this.imgLastMatch.Size = new System.Drawing.Size(786, 411);
            this.imgLastMatch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgLastMatch.TabIndex = 2;
            this.imgLastMatch.TabStop = false;
            // 
            // FormPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabsPreviewer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPreview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Preview";
            this.tabsPreviewer.ResumeLayout(false);
            this.tabLivePreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgLiveFrame)).EndInit();
            this.tabLastMatch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgLastMatch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabsPreviewer;
        private System.Windows.Forms.TabPage tabLivePreview;
        private Emgu.CV.UI.ImageBox imgLiveFrame;
        private System.Windows.Forms.TabPage tabLastMatch;
        private Emgu.CV.UI.ImageBox imgLastMatch;
    }
}