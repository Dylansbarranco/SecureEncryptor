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
            btnBrowse = new Button();
            btnDecrypt = new Button();
            btnEncrypt = new Button();
            txtFile = new TextBox();
            txtPassword = new TextBox();
            SuspendLayout();
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(70, 121);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(144, 23);
            btnBrowse.TabIndex = 0;
            btnBrowse.Text = "Buscar Archivo";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnDecrypt
            // 
            btnDecrypt.Location = new Point(295, 241);
            btnDecrypt.Name = "btnDecrypt";
            btnDecrypt.Size = new Size(75, 23);
            btnDecrypt.TabIndex = 1;
            btnDecrypt.Text = "Descifrar";
            btnDecrypt.UseVisualStyleBackColor = true;
            btnDecrypt.Click += btnDecrypt_Click;
            // 
            // btnEncrypt
            // 
            btnEncrypt.Location = new Point(139, 241);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(75, 23);
            btnEncrypt.TabIndex = 2;
            btnEncrypt.Text = "Cifrar";
            btnEncrypt.UseVisualStyleBackColor = true;
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // txtFile
            // 
            txtFile.AllowDrop = true;
            txtFile.Location = new Point(227, 122);
            txtFile.Name = "txtFile";
            txtFile.Size = new Size(178, 23);
            txtFile.TabIndex = 3;
            txtFile.TextChanged += txtFile_TextChanged;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(100, 185);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(287, 23);
            txtPassword.TabIndex = 4;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(477, 450);
            Controls.Add(txtPassword);
            Controls.Add(txtFile);
            Controls.Add(btnEncrypt);
            Controls.Add(btnDecrypt);
            Controls.Add(btnBrowse);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBrowse;
        private Button btnDecrypt;
        private Button btnEncrypt;
        private TextBox txtFile;
        private TextBox txtPassword;
    }
}
