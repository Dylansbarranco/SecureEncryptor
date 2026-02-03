namespace SecureEncryptor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnBrowse = new Button();
            btnDecrypt = new Button();
            btnEncrypt = new Button();
            txtFile = new TextBox();
            txtPassword = new TextBox();
            lblFile = new Label();
            lblPassword = new Label();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            lblProgressPercent = new Label();
            lblPasswordStrength = new Label();
            lblFileInfo = new Label();
            lblSpeed = new Label();
            btnTogglePassword = new Button();
            chkTopMost = new CheckBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnBrowse
            // 
            btnBrowse.BackColor = Color.FromArgb(0, 120, 215);
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.Location = new Point(410, 20);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(80, 27);
            btnBrowse.TabIndex = 0;
            btnBrowse.Text = "Buscar...";
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnDecrypt
            // 
            btnDecrypt.BackColor = Color.FromArgb(156, 39, 176);
            btnDecrypt.Cursor = Cursors.Hand;
            btnDecrypt.FlatStyle = FlatStyle.Flat;
            btnDecrypt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDecrypt.ForeColor = Color.White;
            btnDecrypt.Location = new Point(270, 210);
            btnDecrypt.Name = "btnDecrypt";
            btnDecrypt.Size = new Size(120, 35);
            btnDecrypt.TabIndex = 1;
            btnDecrypt.Text = "[Descifrar]";
            btnDecrypt.UseVisualStyleBackColor = false;
            btnDecrypt.Click += btnDecrypt_Click;
            // 
            // btnEncrypt
            // 
            btnEncrypt.BackColor = Color.FromArgb(76, 175, 80);
            btnEncrypt.Cursor = Cursors.Hand;
            btnEncrypt.FlatStyle = FlatStyle.Flat;
            btnEncrypt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnEncrypt.ForeColor = Color.White;
            btnEncrypt.Location = new Point(110, 210);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(120, 35);
            btnEncrypt.TabIndex = 2;
            btnEncrypt.Text = "[Cifrar]";
            btnEncrypt.UseVisualStyleBackColor = false;
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // txtFile
            // 
            txtFile.AllowDrop = true;
            txtFile.BorderStyle = BorderStyle.FixedSingle;
            txtFile.Font = new Font("Segoe UI", 9.75F);
            txtFile.Location = new Point(140, 20);
            txtFile.Name = "txtFile";
            txtFile.Size = new Size(260, 25);
            txtFile.TabIndex = 3;
            txtFile.TextChanged += txtFile_TextChanged;
            txtFile.DragDrop += txtFile_DragDrop;
            txtFile.DragEnter += txtFile_DragEnter;
            // 
            // txtPassword
            // 
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Font = new Font("Segoe UI", 9.75F);
            txtPassword.Location = new Point(140, 60);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(260, 25);
            txtPassword.TabIndex = 4;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.TextChanged += txtPassword_TextChanged;
            // 
            // lblFile
            // 
            lblFile.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFile.ForeColor = Color.FromArgb(33, 33, 33);
            lblFile.Location = new Point(20, 20);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(120, 20);
            lblFile.TabIndex = 7;
            lblFile.Text = "Archivo / Carpeta:";
            // 
            // lblPassword
            // 
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(33, 33, 33);
            lblPassword.Location = new Point(20, 60);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(120, 20);
            lblPassword.TabIndex = 6;
            lblPassword.Text = "Contraseña:";
            // 
            // progressBar
            // 
            progressBar.ForeColor = Color.FromArgb(76, 175, 80);
            progressBar.Location = new Point(20, 155);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(470, 25);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 3;
            progressBar.Visible = false;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9.75F);
            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
            lblStatus.Location = new Point(20, 255);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(50, 25);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Listo";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblProgressPercent
            // 
            lblProgressPercent.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblProgressPercent.ForeColor = Color.FromArgb(33, 33, 33);
            lblProgressPercent.Location = new Point(20, 155);
            lblProgressPercent.Name = "lblProgressPercent";
            lblProgressPercent.Size = new Size(470, 25);
            lblProgressPercent.TabIndex = 2;
            lblProgressPercent.Text = "0%";
            lblProgressPercent.TextAlign = ContentAlignment.MiddleCenter;
            lblProgressPercent.Visible = false;
            // 
            // lblPasswordStrength
            // 
            lblPasswordStrength.Font = new Font("Segoe UI", 9F);
            lblPasswordStrength.ForeColor = Color.FromArgb(200, 0, 0);
            lblPasswordStrength.Location = new Point(140, 90);
            lblPasswordStrength.Name = "lblPasswordStrength";
            lblPasswordStrength.Size = new Size(350, 18);
            lblPasswordStrength.TabIndex = 5;
            lblPasswordStrength.Text = "Contraseña débil";
            // 
            // lblFileInfo
            // 
            lblFileInfo.Font = new Font("Segoe UI", 8.5F);
            lblFileInfo.ForeColor = Color.FromArgb(100, 100, 100);
            lblFileInfo.Location = new Point(20, 110);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(470, 40);
            lblFileInfo.TabIndex = 4;
            lblFileInfo.Text = "Seleccione un archivo para ver información";
            // 
            // lblSpeed
            // 
            lblSpeed.Font = new Font("Segoe UI", 8.75F);
            lblSpeed.ForeColor = Color.FromArgb(76, 175, 80);
            lblSpeed.Location = new Point(20, 185);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(470, 18);
            lblSpeed.TabIndex = 1;
            lblSpeed.Visible = false;
            // 
            // btnTogglePassword
            // 
            btnTogglePassword.BackColor = Color.FromArgb(200, 200, 200);
            btnTogglePassword.Cursor = Cursors.Hand;
            btnTogglePassword.FlatStyle = FlatStyle.Flat;
            btnTogglePassword.Font = new Font("Segoe UI", 10F);
            btnTogglePassword.Location = new Point(410, 60);
            btnTogglePassword.Name = "btnTogglePassword";
            btnTogglePassword.Size = new Size(80, 27);
            btnTogglePassword.TabIndex = 5;
            btnTogglePassword.Text = "Ver";
            btnTogglePassword.UseVisualStyleBackColor = false;
            btnTogglePassword.Click += btnTogglePassword_Click;
            // 
            // chkTopMost
            // 
            chkTopMost.AutoSize = true;
            chkTopMost.Checked = true;
            chkTopMost.CheckState = CheckState.Checked;
            chkTopMost.Location = new Point(524, 339);
            chkTopMost.Name = "chkTopMost";
            chkTopMost.Size = new Size(15, 14);
            chkTopMost.TabIndex = 8;
            chkTopMost.UseVisualStyleBackColor = true;
            chkTopMost.CheckedChanged += chkTopMost_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(410, 338);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 9;
            label1.Text = "Siempre enfrente";
            label1.Click += label1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 245, 245);
            ClientSize = new Size(611, 368);
            Controls.Add(label1);
            Controls.Add(chkTopMost);
            Controls.Add(lblStatus);
            Controls.Add(lblSpeed);
            Controls.Add(lblProgressPercent);
            Controls.Add(progressBar);
            Controls.Add(lblFileInfo);
            Controls.Add(lblPasswordStrength);
            Controls.Add(btnTogglePassword);
            Controls.Add(lblPassword);
            Controls.Add(lblFile);
            Controls.Add(txtPassword);
            Controls.Add(txtFile);
            Controls.Add(btnEncrypt);
            Controls.Add(btnDecrypt);
            Controls.Add(btnBrowse);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(520, 300);
            Name = "Form1";
            Text = "SecureEncryptor - Cifrado Seguro";
            TopMost = true;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBrowse;
        private Button btnDecrypt;
        private Button btnEncrypt;
        private TextBox txtFile;
        private TextBox txtPassword;
        private Label lblFile;
        private Label lblPassword;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label lblProgressPercent;
        private Label lblPasswordStrength;
        private Label lblFileInfo;
        private Label lblSpeed;
        private Button btnTogglePassword;
        private CheckBox chkTopMost;
        private Label label1;
    }
}
