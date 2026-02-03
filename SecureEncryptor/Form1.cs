using System.Security.Cryptography;

namespace SecureEncryptor
{
    public partial class Form1 : Form
    {
        private bool isProcessing = false;
        private bool showPassword = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            OpenFileDialog f = new OpenFileDialog();
            f.CheckFileExists = false;

            if (f.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = f.FileName;
                DisplayFileInfo(f.FileName);
            }
        }

        private void DisplayFileInfo(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    FileInfo info = new FileInfo(filePath);
                    string size = FormatFileSize(info.Length);
                    string modified = info.LastWriteTime.ToString("dd/MM/yyyy HH:mm");
                    lblFileInfo.Text = $"[ARCHIVO] {info.Name} | Tamaño: {size} | Modificado: {modified}";
                }
                else if (Directory.Exists(filePath))
                {
                    DirectoryInfo info = new DirectoryInfo(filePath);
                    int fileCount = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories).Length;
                    lblFileInfo.Text = $"[CARPETA] {info.Name} | Archivos: {fileCount}";
                }
            }
            catch (Exception ex)
            {
                lblFileInfo.Text = $"Error: {ex.Message}";
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordStrength();
        }

        private void UpdatePasswordStrength()
        {
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(password))
            {
                lblPasswordStrength.Text = "Ingrese una contraseña";
                lblPasswordStrength.ForeColor = Color.FromArgb(200, 0, 0);
                return;
            }

            // Verificar criterios requeridos por CryptoEngine.IsStrongPassword()
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c));
            bool hasLength16 = password.Length >= 16;
            bool hasLength20 = password.Length >= 20;

            // Contar cuántos criterios cumple
            int criteriasMet = 0;
            if (hasUpper) criteriasMet++;
            if (hasLower) criteriasMet++;
            if (hasDigit) criteriasMet++;
            if (hasSymbol) criteriasMet++;
            if (hasLength16) criteriasMet++;
            if (hasLength20) criteriasMet++;

            // Validación exacta que usa CryptoEngine
            bool isValid = hasUpper && hasLower && hasDigit && hasSymbol && hasLength16;

            if (!isValid)
            {
                if (password.Length < 16)
                {
                    int needed = 16 - password.Length;
                    lblPasswordStrength.Text = $"[!] Necesita {needed} carácter(es) más";
                    lblPasswordStrength.ForeColor = Color.FromArgb(200, 0, 0);
                }
                else
                {
                    var missing = new List<string>();
                    if (!hasUpper) missing.Add("MAYÚSCULA");
                    if (!hasLower) missing.Add("minúscula");
                    if (!hasDigit) missing.Add("número");
                    if (!hasSymbol) missing.Add("símbolo");
                    
                    lblPasswordStrength.Text = $"[!] Falta: {string.Join(", ", missing)}";
                    lblPasswordStrength.ForeColor = Color.FromArgb(200, 0, 0);
                }
            }
            else if (criteriasMet <= 5)
            {
                lblPasswordStrength.Text = "[+] Contraseña fuerte";
                lblPasswordStrength.ForeColor = Color.FromArgb(76, 175, 80);
            }
            else
            {
                lblPasswordStrength.Text = "[++] Contraseña muy fuerte (20+ caracteres)";
                lblPasswordStrength.ForeColor = Color.FromArgb(0, 128, 0);
            }
        }

        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            showPassword = !showPassword;
            txtPassword.UseSystemPasswordChar = !showPassword;
            btnTogglePassword.Text = showPassword ? "Ocultar" : "Ver";
        }

        private async void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                if (isProcessing)
                {
                    MessageBox.Show("Ya hay un proceso en ejecución.");
                    return;
                }

                isProcessing = true;
                DisableControls();
                ShowProgress();

                await Task.Run(() =>
                {
                    if (File.Exists(txtFile.Text))
                    {
                        CryptoEngine.EncryptFile(
                            txtFile.Text,
                            txtPassword.Text,
                            (progress, status) => UpdateProgress(progress, $">> {status}")
                        );

                        DialogResult r = MessageBox.Show(
                            "¿Desea eliminar el archivo original de forma segura?",
                            "Confirmación",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (r == DialogResult.Yes)
                            CryptoEngine.SecureDelete(txtFile.Text);
                    }
                    else if (Directory.Exists(txtFile.Text))
                    {
                        CryptoEngine.EncryptDirectory(
                            txtFile.Text,
                            txtPassword.Text,
                            (progress, status) => UpdateProgress(progress, $">> {status}")
                        );

                        DialogResult r = MessageBox.Show(
                            "¿Desea eliminar los archivos originales de forma segura?",
                            "Confirmación",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (r == DialogResult.Yes)
                        {
                            foreach (string file in Directory.GetFiles(
                                         txtFile.Text,
                                         "*",
                                         SearchOption.AllDirectories))
                            {
                                if (!file.EndsWith(".lol"))
                                    CryptoEngine.SecureDelete(file);
                            }
                        }
                    }
                });

                HideProgress();
                MessageBox.Show("EXITO: Proceso de cifrado completado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtFile.Clear();
                txtPassword.Clear();
                lblFileInfo.Text = "Seleccione un archivo para ver información";
            }
            catch (Exception ex)
            {
                HideProgress();
                MessageBox.Show("ERROR al cifrar:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isProcessing = false;
                EnableControls();
            }
        }

        private async void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFile.Text))
                {
                    MessageBox.Show("Seleccione un archivo o carpeta.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Ingrese la contraseña.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (isProcessing)
                {
                    MessageBox.Show("Ya hay un proceso en ejecución.");
                    return;
                }

                isProcessing = true;
                DisableControls();
                ShowProgress();

                await Task.Run(() =>
                {
                    if (File.Exists(txtFile.Text))
                    {
                        CryptoEngine.DecryptFile(
                            txtFile.Text,
                            txtPassword.Text,
                            (progress, status) => UpdateProgress(progress, $">> {status}")
                        );

                        DialogResult r = MessageBox.Show(
                            "¿Desea eliminar el archivo cifrado de forma segura?",
                            "Confirmación",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (r == DialogResult.Yes)
                            CryptoEngine.SecureDelete(txtFile.Text);
                    }
                    else if (Directory.Exists(txtFile.Text))
                    {
                        CryptoEngine.DecryptDirectory(
                            txtFile.Text,
                            txtPassword.Text,
                            (progress, status) => UpdateProgress(progress, $">> {status}")
                        );

                        DialogResult r = MessageBox.Show(
                            "¿Desea eliminar los archivos cifrados de forma segura?",
                            "Confirmación",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (r == DialogResult.Yes)
                        {
                            foreach (string file in Directory.GetFiles(
                                         txtFile.Text,
                                         "*.lol",
                                         SearchOption.AllDirectories))
                            {
                                CryptoEngine.SecureDelete(file);
                            }
                        }
                    }
                });

                HideProgress();
                MessageBox.Show("EXITO: Proceso de descifrado completado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtFile.Clear();
                txtPassword.Clear();
                lblFileInfo.Text = "Seleccione un archivo para ver información";
            }
            catch (CryptographicException)
            {
                HideProgress();
                MessageBox.Show("ERROR: Contraseña incorrecta o archivo alterado.", "Error de Descifrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                HideProgress();
                MessageBox.Show("ERROR:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isProcessing = false;
                EnableControls();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFile.Text))
            {
                MessageBox.Show("Seleccione un archivo o carpeta.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Ingrese una contraseña.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!CryptoEngine.IsStrongPassword(txtPassword.Text))
            {
                string password = txtPassword.Text;
                var missing = new List<string>();

                if (password.Length < 16)
                    missing.Add($"Mínimo 16 caracteres (tiene {password.Length})");
                
                if (!password.Any(char.IsUpper))
                    missing.Add("Una MAYÚSCULA");
                
                if (!password.Any(char.IsLower))
                    missing.Add("Una minúscula");
                
                if (!password.Any(char.IsDigit))
                    missing.Add("Un número (0-9)");
                
                if (!password.Any(c => !char.IsLetterOrDigit(c)))
                    missing.Add("Un símbolo (!@#$%^&*)");

                string missingText = string.Join("\n- ", missing);
                
                MessageBox.Show(
                    $"ERROR: Contraseña no cumple requisitos.\n\n" +
                    $"Falta:\n- {missingText}\n\n" +
                    $"Ejemplo válido: Abc123!@Def456#$",
                    "Contraseña Débil",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            if (!File.Exists(txtFile.Text) && !Directory.Exists(txtFile.Text))
            {
                MessageBox.Show("Ruta inválida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void UpdateProgress(int progress, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(progress, status)));
                return;
            }

            progressBar.Value = progress;
            lblProgressPercent.Text = $"{progress}%";
            lblStatus.Text = status;
            lblSpeed.Text = status;
        }

        private void ShowProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowProgress()));
                return;
            }

            progressBar.Visible = true;
            lblProgressPercent.Visible = true;
            lblSpeed.Visible = true;
            progressBar.Value = 0;
            lblProgressPercent.Text = "0%";
            lblStatus.Text = "Iniciando...";
            lblSpeed.Text = "";
            lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
        }

        private void HideProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HideProgress()));
                return;
            }

            progressBar.Visible = false;
            lblProgressPercent.Visible = false;
            lblSpeed.Visible = false;
            progressBar.Value = 0;
            lblStatus.Text = "Listo";
            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
        }

        private void DisableControls()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DisableControls()));
                return;
            }

            txtFile.Enabled = false;
            txtPassword.Enabled = false;
            btnBrowse.Enabled = false;
            btnEncrypt.Enabled = false;
            btnDecrypt.Enabled = false;
            btnTogglePassword.Enabled = false;
        }

        private void EnableControls()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => EnableControls()));
                return;
            }

            txtFile.Enabled = true;
            txtPassword.Enabled = true;
            btnBrowse.Enabled = true;
            btnEncrypt.Enabled = true;
            btnDecrypt.Enabled = true;
            btnTogglePassword.Enabled = true;
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
        }

        private void txtFile_DragDrop(object sender, DragEventArgs e)
        {
            string[]? files = (string[]?)e.Data?.GetData(DataFormats.FileDrop);
            if (files?.Length > 0)
            {
                txtFile.Text = files[0];
                DisplayFileInfo(files[0]);
            }
        }
    }
}

