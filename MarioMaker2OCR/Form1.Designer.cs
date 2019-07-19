namespace MarioMaker2OCR
{
    partial class Form1
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
                tmplDeathBig.Dispose();
                tmplDeathPartial.Dispose();
                tmplDeathSmall.Dispose();
                tmplExit.Dispose();
                tmplRestart.Dispose();
                tmplQuit.Dispose();
                if(processor != null) processor.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLevelFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPreviewWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ocrTextBox = new System.Windows.Forms.TextBox();
            this.ocrLabel = new System.Windows.Forms.Label();
            this.deviceLabel = new System.Windows.Forms.Label();
            this.deviceComboBox = new System.Windows.Forms.ComboBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.propertiesButton = new System.Windows.Forms.Button();
            this.outputFolderLabel = new System.Windows.Forms.Label();
            this.outputFolderTextbox = new System.Windows.Forms.TextBox();
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.processStatusIcon = new System.Windows.Forms.ToolStripStatusLabel();
            this.webServerAddressStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.processingLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.numPortLabel = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.resolutionsCombobox = new System.Windows.Forms.ComboBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(622, 35);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearLevelFileToolStripMenuItem,
            this.showPreviewWindowToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // clearLevelFileToolStripMenuItem
            // 
            this.clearLevelFileToolStripMenuItem.Name = "clearLevelFileToolStripMenuItem";
            this.clearLevelFileToolStripMenuItem.Size = new System.Drawing.Size(294, 34);
            this.clearLevelFileToolStripMenuItem.Text = "Clear Level File";
            this.clearLevelFileToolStripMenuItem.Click += new System.EventHandler(this.clearLevelFileToolStripMenuItem_Click_1);
            // 
            // showPreviewWindowToolStripMenuItem
            // 
            this.showPreviewWindowToolStripMenuItem.Name = "showPreviewWindowToolStripMenuItem";
            this.showPreviewWindowToolStripMenuItem.Size = new System.Drawing.Size(294, 34);
            this.showPreviewWindowToolStripMenuItem.Text = "Show Preview Window";
            this.showPreviewWindowToolStripMenuItem.Click += new System.EventHandler(this.showPreviewWindowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(291, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(294, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ocrTextBox
            // 
            this.ocrTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ocrTextBox.Location = new System.Drawing.Point(15, 333);
            this.ocrTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ocrTextBox.Name = "ocrTextBox";
            this.ocrTextBox.ReadOnly = true;
            this.ocrTextBox.Size = new System.Drawing.Size(588, 39);
            this.ocrTextBox.TabIndex = 5;
            // 
            // ocrLabel
            // 
            this.ocrLabel.AutoSize = true;
            this.ocrLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ocrLabel.Location = new System.Drawing.Point(26, 300);
            this.ocrLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ocrLabel.Name = "ocrLabel";
            this.ocrLabel.Size = new System.Drawing.Size(59, 31);
            this.ocrLabel.TabIndex = 10;
            this.ocrLabel.Text = "OCR";
            // 
            // deviceLabel
            // 
            this.deviceLabel.AutoSize = true;
            this.deviceLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deviceLabel.Location = new System.Drawing.Point(43, 52);
            this.deviceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.deviceLabel.Name = "deviceLabel";
            this.deviceLabel.Size = new System.Drawing.Size(82, 31);
            this.deviceLabel.TabIndex = 11;
            this.deviceLabel.Text = "Device";
            // 
            // deviceComboBox
            // 
            this.deviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceComboBox.FormattingEnabled = true;
            this.deviceComboBox.Location = new System.Drawing.Point(135, 55);
            this.deviceComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.deviceComboBox.Name = "deviceComboBox";
            this.deviceComboBox.Size = new System.Drawing.Size(360, 28);
            this.deviceComboBox.TabIndex = 12;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(170, 416);
            this.startButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(112, 35);
            this.startButton.TabIndex = 13;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(318, 416);
            this.stopButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(112, 35);
            this.stopButton.TabIndex = 14;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // propertiesButton
            // 
            this.propertiesButton.Location = new System.Drawing.Point(502, 54);
            this.propertiesButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.Size = new System.Drawing.Size(100, 35);
            this.propertiesButton.TabIndex = 15;
            this.propertiesButton.Text = "Properties";
            this.propertiesButton.UseVisualStyleBackColor = true;
            this.propertiesButton.Click += new System.EventHandler(this.propertiesButton_Click);
            // 
            // outputFolderLabel
            // 
            this.outputFolderLabel.AutoSize = true;
            this.outputFolderLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputFolderLabel.Location = new System.Drawing.Point(38, 199);
            this.outputFolderLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.outputFolderLabel.Name = "outputFolderLabel";
            this.outputFolderLabel.Size = new System.Drawing.Size(87, 62);
            this.outputFolderLabel.TabIndex = 17;
            this.outputFolderLabel.Text = "Output\r\n Folder";
            // 
            // outputFolderTextbox
            // 
            this.outputFolderTextbox.Enabled = false;
            this.outputFolderTextbox.Location = new System.Drawing.Point(133, 199);
            this.outputFolderTextbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.outputFolderTextbox.Multiline = true;
            this.outputFolderTextbox.Name = "outputFolderTextbox";
            this.outputFolderTextbox.Size = new System.Drawing.Size(361, 76);
            this.outputFolderTextbox.TabIndex = 18;
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(502, 199);
            this.selectFolderButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(100, 35);
            this.selectFolderButton.TabIndex = 19;
            this.selectFolderButton.Text = "Select Folder";
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.processStatusIcon,
            this.webServerAddressStatusLabel,
            this.toolStripStatusLabel1,
            this.processingLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 463);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(622, 35);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 20;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // processStatusIcon
            // 
            this.processStatusIcon.AutoToolTip = true;
            this.processStatusIcon.BackColor = System.Drawing.Color.Red;
            this.processStatusIcon.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.processStatusIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.processStatusIcon.Margin = new System.Windows.Forms.Padding(4, 3, 0, 2);
            this.processStatusIcon.Name = "processStatusIcon";
            this.processStatusIcon.Padding = new System.Windows.Forms.Padding(0, 0, 17, 0);
            this.processStatusIcon.Size = new System.Drawing.Size(17, 30);
            this.processStatusIcon.ToolTipText = "Process status";
            // 
            // webServerAddressStatusLabel
            // 
            this.webServerAddressStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.webServerAddressStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.webServerAddressStatusLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.webServerAddressStatusLabel.IsLink = true;
            this.webServerAddressStatusLabel.Margin = new System.Windows.Forms.Padding(5, 3, 0, 2);
            this.webServerAddressStatusLabel.Name = "webServerAddressStatusLabel";
            this.webServerAddressStatusLabel.Size = new System.Drawing.Size(0, 30);
            this.webServerAddressStatusLabel.Click += new System.EventHandler(this.webServerAddressStatusLabel_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(361, 28);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // processingLabel
            // 
            this.processingLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.processingLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processingLabel.Name = "processingLabel";
            this.processingLabel.Size = new System.Drawing.Size(166, 28);
            this.processingLabel.Text = "Processing Level...";
            this.processingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.processingLabel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.processingLabel.Visible = false;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Tool Tip";
            // 
            // numPortLabel
            // 
            this.numPortLabel.AutoSize = true;
            this.numPortLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPortLabel.Location = new System.Drawing.Point(70, 158);
            this.numPortLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.numPortLabel.Name = "numPortLabel";
            this.numPortLabel.Size = new System.Drawing.Size(55, 31);
            this.numPortLabel.TabIndex = 22;
            this.numPortLabel.Text = "Port";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(134, 158);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(126, 26);
            this.numPort.TabIndex = 23;
            this.numPort.Value = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            // 
            // resolutionsCombobox
            // 
            this.resolutionsCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolutionsCombobox.FormattingEnabled = true;
            this.resolutionsCombobox.Location = new System.Drawing.Point(134, 106);
            this.resolutionsCombobox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resolutionsCombobox.Name = "resolutionsCombobox";
            this.resolutionsCombobox.Size = new System.Drawing.Size(360, 28);
            this.resolutionsCombobox.TabIndex = 25;
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResolution.Location = new System.Drawing.Point(4, 106);
            this.lblResolution.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(121, 31);
            this.lblResolution.TabIndex = 24;
            this.lblResolution.Text = "Resolution";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 498);
            this.Controls.Add(this.resolutionsCombobox);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.numPort);
            this.Controls.Add(this.numPortLabel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.selectFolderButton);
            this.Controls.Add(this.outputFolderTextbox);
            this.Controls.Add(this.outputFolderLabel);
            this.Controls.Add(this.propertiesButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.deviceComboBox);
            this.Controls.Add(this.deviceLabel);
            this.Controls.Add(this.ocrLabel);
            this.Controls.Add(this.ocrTextBox);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Mario Maker 2 OCR";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TextBox ocrTextBox;
        private System.Windows.Forms.Label ocrLabel;
        private System.Windows.Forms.ToolStripMenuItem clearLevelFileToolStripMenuItem;
        private System.Windows.Forms.Label deviceLabel;
        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button propertiesButton;
        private System.Windows.Forms.Label outputFolderLabel;
        private System.Windows.Forms.TextBox outputFolderTextbox;
        private System.Windows.Forms.Button selectFolderButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel processStatusIcon;
        private System.Windows.Forms.ToolStripStatusLabel processingLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem showPreviewWindowToolStripMenuItem;
        private System.Windows.Forms.Label numPortLabel;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel webServerAddressStatusLabel;
        private System.Windows.Forms.ComboBox resolutionsCombobox;
        private System.Windows.Forms.Label lblResolution;
    }
}

