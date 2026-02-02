using System.Security.Cryptography;

namespace SecureEncryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.CheckFileExists = false;

            if (f.ShowDialog() == DialogResult.OK)
                txtFile.Text = f.FileName;
        }
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFile.Text))
                {
                    MessageBox.Show("Seleccione un archivo o carpeta.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Ingrese una contraseña.");
                    return;
                }

                if (!CryptoEngine.IsStrongPassword(txtPassword.Text))
                {
                    MessageBox.Show(
                        "Contraseña débil.\n\n" +
                        "Debe tener mínimo 16 caracteres,\n" +
                        "mayúscula, minúscula, número y símbolo."
                    );
                    return;
                }

                if (File.Exists(txtFile.Text))
                {
                    CryptoEngine.EncryptFile(txtFile.Text, txtPassword.Text);

                    DialogResult r = MessageBox.Show(
                        "¿Desea eliminar el archivo original?",
                        "Confirmación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (r == DialogResult.Yes)
                        File.Delete(txtFile.Text);
                }

                else if (Directory.Exists(txtFile.Text))
                {
                    CryptoEngine.EncryptDirectory(txtFile.Text, txtPassword.Text);

                    DialogResult r = MessageBox.Show(
                        "¿Desea eliminar los archivos originales?",
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
                                File.Delete(file);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Ruta inválida.");
                    return;
                }

                MessageBox.Show("Proceso de cifrado completado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cifrar:\n" + ex.Message);
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFile.Text))
                {
                    MessageBox.Show("Seleccione un archivo o carpeta.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Ingrese la contraseña.");
                    return;
                }

                if (File.Exists(txtFile.Text))
                {
                    CryptoEngine.DecryptFile(txtFile.Text, txtPassword.Text);

                    DialogResult r = MessageBox.Show(
                        "¿Desea eliminar el archivo cifrado?",
                        "Confirmación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (r == DialogResult.Yes)
                        File.Delete(txtFile.Text);
                }
                else if (Directory.Exists(txtFile.Text))
                {
                    CryptoEngine.EncryptDirectory(txtFile.Text, txtPassword.Text);

                    DialogResult r = MessageBox.Show(
                        "¿Desea eliminar los archivos originales?",
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
                                File.Delete(file);
                        }
                    }
                }

                else
                {
                    MessageBox.Show("Ruta inválida.");
                    return;
                }

                MessageBox.Show("Proceso de descifrado completado.");
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Contraseña incorrecta o archivo alterado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message);
            }
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        private void txtFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            txtFile.Text = files[0];
        }


    }
}

