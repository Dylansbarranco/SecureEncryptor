using System;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SecureEncryptor
{
    public static class CryptoEngine
    {
        // Parámetros criptográficos
        private const int SaltSize = 16;
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const int KeySize = 32;     // 256 bits
        private const int Iterations = 300000;

  
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


        // DERIVAR CLAVE DESDE CONTRASEÑA
       
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

        // BORRADO SEGURO DE ARCHIVO
        public static void SecureDelete(string filePath, int overwrites = 3)
        {
            if (!File.Exists(filePath))
                return;

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                long fileSize = fileInfo.Length;

                // Múltiples sobrescrituras para borrado seguro
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[1024 * 64]; // 64 KB buffer
                    
                    for (int pass = 0; pass < overwrites; pass++)
                    {
                        // Primer pass: ceros, segundo: unos, tercero: datos aleatorios
                        if (pass == 0)
                            Array.Clear(buffer, 0, buffer.Length);
                        else if (pass == 1)
                            Array.Fill(buffer, (byte)0xFF);
                        else
                            RandomNumberGenerator.Fill(buffer);

                        fs.Seek(0, SeekOrigin.Begin);
                        long remaining = fileSize;
                        while (remaining > 0)
                        {
                            int toWrite = (int)Math.Min(buffer.Length, remaining);
                            fs.Write(buffer, 0, toWrite);
                            remaining -= toWrite;
                        }
                        fs.Flush();
                    }
                }

                File.Delete(filePath);
            }
            catch
            {
                // Si falla borrado seguro, intenta borrado normal
                try { File.Delete(filePath); } catch { }
            }
        }

        // CIFRAR ARCHIVO

        public static void EncryptFile(string inputFile, string password, Action<int, string>? onProgress = null)
        {
            if (!IsStrongPassword(password))
                throw new Exception("Contraseña insegura.");

            Stopwatch sw = Stopwatch.StartNew();
            FileInfo fileInfo = new FileInfo(inputFile);
            long fileSize = fileInfo.Length;

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

            sw.Stop();
            double speedMBps = (fileSize / (1024.0 * 1024.0)) / sw.Elapsed.TotalSeconds;
            onProgress?.Invoke(50, $"Cifrado: {speedMBps:F2} MB/s");

            // Guardar extensión original
            string extension = Path.GetExtension(inputFile);
            byte[] extBytes = System.Text.Encoding.UTF8.GetBytes(extension);
            byte[] extSize = BitConverter.GetBytes(extBytes.Length);

            string dir = Path.GetDirectoryName(inputFile)!;
            string name = Path.GetFileNameWithoutExtension(inputFile);
            string newName = $"{name}_{Guid.NewGuid().ToString("N")[..8]}.lol";
            string outFile = Path.Combine(dir, newName);

            using (var fs = new FileStream(outFile, FileMode.Create))
            {
                fs.Write(salt);
                fs.Write(nonce);
                fs.Write(tag);
                fs.Write(extSize);     // tamaño extensión
                fs.Write(extBytes);    // extensión
                fs.Write(cipher);      // datos
            }

            onProgress?.Invoke(100, "Cifrado completado");

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }


        // DESCIFRAR ARCHIVO

        public static void DecryptFile(string encryptedFile, string password, Action<int, string>? onProgress = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            FileInfo fileInfo = new FileInfo(encryptedFile);
            long fileSize = fileInfo.Length;

            using FileStream fs = new FileStream(encryptedFile, FileMode.Open);

            byte[] salt = new byte[SaltSize];
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];

            fs.Read(salt);
            fs.Read(nonce);
            fs.Read(tag);

            onProgress?.Invoke(25, "Leyendo metadatos...");

            // Leer extensión
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

            sw.Stop();
            double speedMBps = (fileSize / (1024.0 * 1024.0)) / sw.Elapsed.TotalSeconds;
            onProgress?.Invoke(75, $"Guardando: {speedMBps:F2} MB/s");

            string output = encryptedFile.Replace(".lol", "") + extension;
            File.WriteAllBytes(output, plain);

            onProgress?.Invoke(100, "Descifrado completado");

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }



        // CIFRAR CARPETA COMPLETA

        public static void EncryptDirectory(string folderPath, string password, Action<int, string>? onProgress = null)
        {
            string[] files = Directory.GetFiles(
                folderPath,
                "*",
                SearchOption.AllDirectories
            );

            int totalFiles = files.Length;
            for (int i = 0; i < files.Length; i++)
            {
                // Evitar volver a cifrar .lol
                if (!files[i].EndsWith(".lol"))
                {
                    EncryptFile(files[i], password);
                    int progress = (int)((i + 1) / (double)totalFiles * 100);
                    onProgress?.Invoke(progress, $"Cifrando archivo {i + 1}/{totalFiles}");
                }
            }
        }

  
        // DESCIFRAR CARPETA COMPLETA
    
        public static void DecryptDirectory(string folderPath, string password, Action<int, string>? onProgress = null)
        {
            string[] files = Directory.GetFiles(
                folderPath,
                "*.lol",
                SearchOption.AllDirectories
            );

            int totalFiles = files.Length;
            for (int i = 0; i < files.Length; i++)
            {
                DecryptFile(files[i], password);
                int progress = (int)((i + 1) / (double)totalFiles * 100);
                onProgress?.Invoke(progress, $"Descifrando archivo {i + 1}/{totalFiles}");
            }
        }
    }
}
