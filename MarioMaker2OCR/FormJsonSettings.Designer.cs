namespace MarioMaker2OCR
{
    partial class FormJsonSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.newLevelDetectedCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.exitCheckbox = new System.Windows.Forms.CheckBox();
            this.skipCheckbox = new System.Windows.Forms.CheckBox();
            this.gameoverCheckbox = new System.Windows.Forms.CheckBox();
            this.stopCaptureCheckbox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Update JSON file on";
            // 
            // newLevelDetectedCheckbox
            // 
            this.newLevelDetectedCheckbox.AutoSize = true;
            this.newLevelDetectedCheckbox.Checked = true;
            this.newLevelDetectedCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.newLevelDetectedCheckbox.Enabled = false;
            this.newLevelDetectedCheckbox.Location = new System.Drawing.Point(41, 34);
            this.newLevelDetectedCheckbox.Name = "newLevelDetectedCheckbox";
            this.newLevelDetectedCheckbox.Size = new System.Drawing.Size(138, 21);
            this.newLevelDetectedCheckbox.TabIndex = 1;
            this.newLevelDetectedCheckbox.Text = "New level detected";
            this.newLevelDetectedCheckbox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Clear JSON file on";
            // 
            // exitCheckbox
            // 
            this.exitCheckbox.AutoSize = true;
            this.exitCheckbox.Location = new System.Drawing.Point(41, 110);
            this.exitCheckbox.Name = "exitCheckbox";
            this.exitCheckbox.Size = new System.Drawing.Size(120, 21);
            this.exitCheckbox.TabIndex = 3;
            this.exitCheckbox.Text = "Exit or quit level";
            this.exitCheckbox.UseVisualStyleBackColor = true;
            // 
            // skipCheckbox
            // 
            this.skipCheckbox.AutoSize = true;
            this.skipCheckbox.Location = new System.Drawing.Point(41, 137);
            this.skipCheckbox.Name = "skipCheckbox";
            this.skipCheckbox.Size = new System.Drawing.Size(81, 21);
            this.skipCheckbox.TabIndex = 4;
            this.skipCheckbox.Text = "Skip level";
            this.skipCheckbox.UseVisualStyleBackColor = true;
            // 
            // gameoverCheckbox
            // 
            this.gameoverCheckbox.AutoSize = true;
            this.gameoverCheckbox.Location = new System.Drawing.Point(41, 165);
            this.gameoverCheckbox.Name = "gameoverCheckbox";
            this.gameoverCheckbox.Size = new System.Drawing.Size(87, 21);
            this.gameoverCheckbox.TabIndex = 5;
            this.gameoverCheckbox.Text = "Gameover";
            this.gameoverCheckbox.UseVisualStyleBackColor = true;
            // 
            // stopCaptureCheckbox
            // 
            this.stopCaptureCheckbox.AutoSize = true;
            this.stopCaptureCheckbox.Checked = true;
            this.stopCaptureCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopCaptureCheckbox.Location = new System.Drawing.Point(41, 193);
            this.stopCaptureCheckbox.Name = "stopCaptureCheckbox";
            this.stopCaptureCheckbox.Size = new System.Drawing.Size(172, 21);
            this.stopCaptureCheckbox.TabIndex = 6;
            this.stopCaptureCheckbox.Text = "Stopping Capture Device";
            this.stopCaptureCheckbox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(53, 241);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 27);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(152, 241);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 27);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormJsonSettings
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(280, 286);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.stopCaptureCheckbox);
            this.Controls.Add(this.gameoverCheckbox);
            this.Controls.Add(this.skipCheckbox);
            this.Controls.Add(this.exitCheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.newLevelDetectedCheckbox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormJsonSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "JSON Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox newLevelDetectedCheckbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox exitCheckbox;
        private System.Windows.Forms.CheckBox skipCheckbox;
        private System.Windows.Forms.CheckBox gameoverCheckbox;
        private System.Windows.Forms.CheckBox stopCaptureCheckbox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}