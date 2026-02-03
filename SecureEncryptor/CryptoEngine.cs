using System;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SecureEncryptor
{
    public static class CryptoEngine
    {
        // PARÁMETROS
        private const int SaltSize = 16;
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 300000;
        private const string EncryptedExtension = ".sef";

        // VALIDAR CONTRASEÑA FUERTE

        public static bool IsStrongPassword(string password)
        {
            if (password.Length < 16)
                return false;

            bool upper = false, lower = false, digit = false, symbol = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) upper = true;
                else if (char.IsLower(c)) lower = true;
                else if (char.IsDigit(c)) digit = true;
                else symbol = true;
            }

            return upper && lower && digit && symbol;
        }


        // DERIVAR CLAVE
  
        private static byte[] DeriveKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize
            );
        }

   
        // BORRADO SEGURO

        public static void SecureDelete(string filePath, int overwrites = 3)
        {
            if (!File.Exists(filePath))
                return;

            try
            {
                FileInfo fi = new FileInfo(filePath);
                long size = fi.Length;

                using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write);

                byte[] buffer = new byte[64 * 1024];

                for (int pass = 0; pass < overwrites; pass++)
                {
                    if (pass == 0)
                        Array.Clear(buffer);
                    else if (pass == 1)
                        Array.Fill(buffer, (byte)0xFF);
                    else
                        RandomNumberGenerator.Fill(buffer);

                    fs.Seek(0, SeekOrigin.Begin);
                    long remaining = size;

                    while (remaining > 0)
                    {
                        int chunk = (int)Math.Min(buffer.Length, remaining);
                        fs.Write(buffer, 0, chunk);
                        remaining -= chunk;
                    }
                    fs.Flush();
                }

                File.Delete(filePath);
            }
            catch
            {
                try { File.Delete(filePath); } catch { }
            }
        }


        // CIFRAR ARCHIVO

        public static void EncryptFile(string inputFile, string password, Action<int, string>? onProgress = null)
        {
            if (!IsStrongPassword(password))
                throw new Exception("Contraseña insegura.");

            Stopwatch sw = Stopwatch.StartNew();

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] key = DeriveKey(password, salt);

            byte[] plain = File.ReadAllBytes(inputFile);
            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[TagSize];

            using (var aes = new AesGcm(key, TagSize))
            {
                aes.Encrypt(nonce, plain, cipher, tag);
            }

            onProgress?.Invoke(50, "Cifrando...");

            // Guardar extensión original
            string extension = Path.GetExtension(inputFile);
            byte[] extBytes = System.Text.Encoding.UTF8.GetBytes(extension);
            byte[] extSize = BitConverter.GetBytes(extBytes.Length);

            string dir = Path.GetDirectoryName(inputFile)!;
            string newName = $"{Guid.NewGuid():N}{EncryptedExtension}";
            string outFile = Path.Combine(dir, newName);

            using (FileStream fs = new FileStream(outFile, FileMode.Create))
            {
                fs.Write(salt);
                fs.Write(nonce);
                fs.Write(tag);
                fs.Write(extSize);
                fs.Write(extBytes);
                fs.Write(cipher);
            }

            sw.Stop();
            onProgress?.Invoke(100, "Cifrado completado");

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }

  
        // DESCIFRAR ARCHIVO

        public static void DecryptFile(string encryptedFile, string password, Action<int, string>? onProgress = null)
        {
            using FileStream fs = new FileStream(encryptedFile, FileMode.Open);

            byte[] salt = new byte[SaltSize];
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];

            fs.Read(salt);
            fs.Read(nonce);
            fs.Read(tag);

            byte[] extSizeBytes = new byte[4];
            fs.Read(extSizeBytes);
            int extSize = BitConverter.ToInt32(extSizeBytes);

            byte[] extBytes = new byte[extSize];
            fs.Read(extBytes);
            string extension = System.Text.Encoding.UTF8.GetString(extBytes);

            byte[] cipher = new byte[fs.Length - fs.Position];
            fs.Read(cipher);

            onProgress?.Invoke(50, "Descifrando...");

            byte[] key = DeriveKey(password, salt);
            byte[] plain = new byte[cipher.Length];

            using (var aes = new AesGcm(key, TagSize))
            {
                aes.Decrypt(nonce, cipher, tag, plain);
            }

            string output = Path.Combine(
                Path.GetDirectoryName(encryptedFile)!,
                Path.GetFileNameWithoutExtension(encryptedFile) + extension
            );

            File.WriteAllBytes(output, plain);

            onProgress?.Invoke(100, "Descifrado completado");

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }

   
        // CIFRAR CARPETA
    
        public static void EncryptDirectory(string folderPath, string password, Action<int, string>? onProgress = null)
        {
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            int total = files.Length;

            for (int i = 0; i < total; i++)
            {
                if (!files[i].EndsWith(EncryptedExtension))
                {
                    EncryptFile(files[i], password);
                    int progress = (int)((i + 1) / (double)total * 100);
                    onProgress?.Invoke(progress, $"Cifrando {i + 1}/{total}");
                }
            }
        }
        // DESCIFRAR CARPETA
        
        public static void DecryptDirectory(string folderPath, string password, Action<int, string>? onProgress = null)
        {
            string[] files = Directory.GetFiles(folderPath, "*" + EncryptedExtension, SearchOption.AllDirectories);

            int total = files.Length;

            for (int i = 0; i < total; i++)
            {
                DecryptFile(files[i], password);
                int progress = (int)((i + 1) / (double)total * 100);
                onProgress?.Invoke(progress, $"Descifrando {i + 1}/{total}");
            }
        }
    }
}
