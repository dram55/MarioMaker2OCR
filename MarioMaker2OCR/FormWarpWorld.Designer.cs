namespace MarioMaker2OCR
{
    partial class FormWarpWorld
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
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblToken = new System.Windows.Forms.Label();
            this.lblWarpBarURL = new System.Windows.Forms.Label();
            this.txtWarpBarURL = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.chkWarpWorld = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.Location = new System.Drawing.Point(15, 158);
            this.lblUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(117, 31);
            this.lblUsername.TabIndex = 12;
            this.lblUsername.Text = "Username";
            // 
            // lblToken
            // 
            this.lblToken.AutoSize = true;
            this.lblToken.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToken.Location = new System.Drawing.Point(59, 199);
            this.lblToken.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblToken.Name = "lblToken";
            this.lblToken.Size = new System.Drawing.Size(73, 31);
            this.lblToken.TabIndex = 13;
            this.lblToken.Text = "Token";
            // 
            // lblWarpBarURL
            // 
            this.lblWarpBarURL.AutoSize = true;
            this.lblWarpBarURL.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarpBarURL.Location = new System.Drawing.Point(15, 69);
            this.lblWarpBarURL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWarpBarURL.Name = "lblWarpBarURL";
            this.lblWarpBarURL.Size = new System.Drawing.Size(154, 31);
            this.lblWarpBarURL.TabIndex = 14;
            this.lblWarpBarURL.Text = "Warp Bar URL";
            // 
            // txtWarpBarURL
            // 
            this.txtWarpBarURL.Location = new System.Drawing.Point(26, 112);
            this.txtWarpBarURL.Name = "txtWarpBarURL";
            this.txtWarpBarURL.Size = new System.Drawing.Size(626, 26);
            this.txtWarpBarURL.TabIndex = 15;
            this.txtWarpBarURL.TextChanged += new System.EventHandler(this.TxtWarpBarURL_TextChanged);
            // 
            // txtUsername
            // 
            this.txtUsername.Enabled = false;
            this.txtUsername.Location = new System.Drawing.Point(155, 163);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(497, 26);
            this.txtUsername.TabIndex = 16;
            // 
            // txtToken
            // 
            this.txtToken.Enabled = false;
            this.txtToken.Location = new System.Drawing.Point(155, 204);
            this.txtToken.Name = "txtToken";
            this.txtToken.Size = new System.Drawing.Size(497, 26);
            this.txtToken.TabIndex = 17;
            // 
            // chkWarpWorld
            // 
            this.chkWarpWorld.AutoSize = true;
            this.chkWarpWorld.Location = new System.Drawing.Point(21, 28);
            this.chkWarpWorld.Name = "chkWarpWorld";
            this.chkWarpWorld.Size = new System.Drawing.Size(598, 24);
            this.chkWarpWorld.TabIndex = 18;
            this.chkWarpWorld.Text = "Automatically mark the current level in your queue as a win (cleared) or loss (ex" +
    "it)";
            this.chkWarpWorld.UseVisualStyleBackColor = true;
            this.chkWarpWorld.CheckedChanged += new System.EventHandler(this.ChkWarpWorld_CheckedChanged);
            // 
            // FormWarpWorld
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 267);
            this.Controls.Add(this.chkWarpWorld);
            this.Controls.Add(this.txtToken);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtWarpBarURL);
            this.Controls.Add(this.lblWarpBarURL);
            this.Controls.Add(this.lblToken);
            this.Controls.Add(this.lblUsername);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWarpWorld";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Warp World Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblToken;
        private System.Windows.Forms.Label lblWarpBarURL;
        private System.Windows.Forms.TextBox txtWarpBarURL;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.CheckBox chkWarpWorld;
    }
}