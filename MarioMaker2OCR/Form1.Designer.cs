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
            this.showPreviewWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.warpWorldSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ocrTextBox = new System.Windows.Forms.TextBox();
            this.ocrLabel = new System.Windows.Forms.Label();
            this.deviceLabel = new System.Windows.Forms.Label();
            this.deviceComboBox = new System.Windows.Forms.ComboBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.propertiesButton = new System.Windows.Forms.Button();
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
            this.langNeutralcheckBox = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.jsonOutputFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectOutputFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(415, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showPreviewWindowToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // showPreviewWindowToolStripMenuItem
            // 
            this.showPreviewWindowToolStripMenuItem.Name = "showPreviewWindowToolStripMenuItem";
            this.showPreviewWindowToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.showPreviewWindowToolStripMenuItem.Text = "Show Preview Window";
            this.showPreviewWindowToolStripMenuItem.Click += new System.EventHandler(this.showPreviewWindowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(191, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.warpWorldSettingsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.jsonOutputFileToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // warpWorldSettingsToolStripMenuItem
            // 
            this.warpWorldSettingsToolStripMenuItem.Name = "warpWorldSettingsToolStripMenuItem";
            this.warpWorldSettingsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.warpWorldSettingsToolStripMenuItem.Text = "Warp World Settings";
            this.warpWorldSettingsToolStripMenuItem.Click += new System.EventHandler(this.WarpWorldSettingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem2.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ocrTextBox
            // 
            this.ocrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ocrTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ocrTextBox.Location = new System.Drawing.Point(10, 194);
            this.ocrTextBox.Name = "ocrTextBox";
            this.ocrTextBox.ReadOnly = true;
            this.ocrTextBox.Size = new System.Drawing.Size(393, 29);
            this.ocrTextBox.TabIndex = 5;
            // 
            // ocrLabel
            // 
            this.ocrLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ocrLabel.AutoSize = true;
            this.ocrLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ocrLabel.Location = new System.Drawing.Point(17, 173);
            this.ocrLabel.Name = "ocrLabel";
            this.ocrLabel.Size = new System.Drawing.Size(38, 20);
            this.ocrLabel.TabIndex = 10;
            this.ocrLabel.Text = "OCR";
            // 
            // deviceLabel
            // 
            this.deviceLabel.AutoSize = true;
            this.deviceLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deviceLabel.Location = new System.Drawing.Point(29, 34);
            this.deviceLabel.Name = "deviceLabel";
            this.deviceLabel.Size = new System.Drawing.Size(54, 20);
            this.deviceLabel.TabIndex = 11;
            this.deviceLabel.Text = "Device";
            // 
            // deviceComboBox
            // 
            this.deviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceComboBox.FormattingEnabled = true;
            this.deviceComboBox.Location = new System.Drawing.Point(90, 36);
            this.deviceComboBox.Name = "deviceComboBox";
            this.deviceComboBox.Size = new System.Drawing.Size(241, 21);
            this.deviceComboBox.TabIndex = 12;
            this.deviceComboBox.SelectedIndexChanged += new System.EventHandler(this.deviceComboBox_SelectedIndexChanged);
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startButton.Location = new System.Drawing.Point(113, 246);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 13;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(212, 246);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 14;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // propertiesButton
            // 
            this.propertiesButton.Location = new System.Drawing.Point(335, 35);
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.Size = new System.Drawing.Size(67, 23);
            this.propertiesButton.TabIndex = 15;
            this.propertiesButton.Text = "Properties";
            this.propertiesButton.UseVisualStyleBackColor = true;
            this.propertiesButton.Click += new System.EventHandler(this.propertiesButton_Click);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 280);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(415, 22);
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
            this.processStatusIcon.Size = new System.Drawing.Size(17, 17);
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
            this.webServerAddressStatusLabel.Size = new System.Drawing.Size(0, 17);
            this.webServerAddressStatusLabel.Click += new System.EventHandler(this.webServerAddressStatusLabel_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(374, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // processingLabel
            // 
            this.processingLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.processingLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processingLabel.Name = "processingLabel";
            this.processingLabel.Size = new System.Drawing.Size(113, 17);
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
            this.numPortLabel.Location = new System.Drawing.Point(47, 103);
            this.numPortLabel.Name = "numPortLabel";
            this.numPortLabel.Size = new System.Drawing.Size(35, 20);
            this.numPortLabel.TabIndex = 22;
            this.numPortLabel.Text = "Port";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(89, 103);
            this.numPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.numPort.Size = new System.Drawing.Size(84, 20);
            this.numPort.TabIndex = 23;
            this.numPort.Value = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            this.numPort.ValueChanged += new System.EventHandler(this.numPort_ValueChanged);
            // 
            // resolutionsCombobox
            // 
            this.resolutionsCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolutionsCombobox.FormattingEnabled = true;
            this.resolutionsCombobox.Location = new System.Drawing.Point(89, 69);
            this.resolutionsCombobox.Name = "resolutionsCombobox";
            this.resolutionsCombobox.Size = new System.Drawing.Size(241, 21);
            this.resolutionsCombobox.TabIndex = 25;
            this.resolutionsCombobox.SelectedIndexChanged += new System.EventHandler(this.resolutionsCombobox_SelectedIndexChanged);
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResolution.Location = new System.Drawing.Point(3, 69);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(79, 20);
            this.lblResolution.TabIndex = 24;
            this.lblResolution.Text = "Resolution";
            // 
            // langNeutralcheckBox
            // 
            this.langNeutralcheckBox.AutoSize = true;
            this.langNeutralcheckBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.langNeutralcheckBox.Location = new System.Drawing.Point(89, 140);
            this.langNeutralcheckBox.Name = "langNeutralcheckBox";
            this.langNeutralcheckBox.Size = new System.Drawing.Size(304, 24);
            this.langNeutralcheckBox.TabIndex = 26;
            this.langNeutralcheckBox.Text = "Detect multiple languages (experimental)";
            this.langNeutralcheckBox.UseVisualStyleBackColor = true;
            this.langNeutralcheckBox.CheckedChanged += new System.EventHandler(this.langNeutralcheckBox_CheckedChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(179, 6);
            // 
            // jsonOutputFileToolStripMenuItem
            // 
            this.jsonOutputFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1,
            this.selectOutputFolderToolStripMenuItem,
            this.toolStripMenuItem3,
            this.clearFileToolStripMenuItem});
            this.jsonOutputFileToolStripMenuItem.Name = "jsonOutputFileToolStripMenuItem";
            this.jsonOutputFileToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.jsonOutputFileToolStripMenuItem.Text = "Json Output File...";
            // 
            // selectOutputFolderToolStripMenuItem
            // 
            this.selectOutputFolderToolStripMenuItem.Name = "selectOutputFolderToolStripMenuItem";
            this.selectOutputFolderToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.selectOutputFolderToolStripMenuItem.Text = "Select Output Folder";
            this.selectOutputFolderToolStripMenuItem.Click += new System.EventHandler(this.selectOutputFolderToolStripMenuItem_Click);
            // 
            // clearFileToolStripMenuItem
            // 
            this.clearFileToolStripMenuItem.Name = "clearFileToolStripMenuItem";
            this.clearFileToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.clearFileToolStripMenuItem.Text = "Clear File";
            this.clearFileToolStripMenuItem.Click += new System.EventHandler(this.clearFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(179, 6);
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
            this.settingsToolStripMenuItem1.Text = "Settings...";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 302);
            this.Controls.Add(this.langNeutralcheckBox);
            this.Controls.Add(this.resolutionsCombobox);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.numPort);
            this.Controls.Add(this.numPortLabel);
            this.Controls.Add(this.statusStrip1);
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
        private System.Windows.Forms.Label deviceLabel;
        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button propertiesButton;
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
        private System.Windows.Forms.CheckBox langNeutralcheckBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem warpWorldSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem jsonOutputFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectOutputFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    }
}

