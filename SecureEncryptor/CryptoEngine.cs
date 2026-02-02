using System;
using System.IO;
using System.Security.Cryptography;

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


        // CIFRAR ARCHIVO

        public static void EncryptFile(string inputFile, string password)
        {
            if (!IsStrongPassword(password))
                throw new Exception("Contraseña insegura.");

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

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }


        // DESCIFRAR ARCHIVO

        public static void DecryptFile(string encryptedFile, string password)
        {
            using FileStream fs = new FileStream(encryptedFile, FileMode.Open);

            byte[] salt = new byte[SaltSize];
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];

            fs.Read(salt);
            fs.Read(nonce);
            fs.Read(tag);

            // Leer extensión
            byte[] extSizeBytes = new byte[4];
            fs.Read(extSizeBytes);
            int extSize = BitConverter.ToInt32(extSizeBytes);

            byte[] extBytes = new byte[extSize];
            fs.Read(extBytes);
            string extension = System.Text.Encoding.UTF8.GetString(extBytes);

            byte[] cipher = new byte[fs.Length - fs.Position];
            fs.Read(cipher);

            byte[] key = DeriveKey(password, salt);
            byte[] plain = new byte[cipher.Length];

            using (var aes = new AesGcm(key, TagSize))
            {
                aes.Decrypt(nonce, cipher, tag, plain);
            }

            string output = encryptedFile.Replace(".lol", "") + extension;
            File.WriteAllBytes(output, plain);

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }



        // CIFRAR CARPETA COMPLETA

        public static void EncryptDirectory(string folderPath, string password)
        {
            string[] files = Directory.GetFiles(
                folderPath,
                "*",
                SearchOption.AllDirectories
            );

            foreach (string file in files)
            {
                // Evitar volver a cifrar .lol
                if (!file.EndsWith(".lol"))
                    EncryptFile(file, password);
            }
        }

  
        // DESCIFRAR CARPETA COMPLETA
    
        public static void DecryptDirectory(string folderPath, string password)
        {
            string[] files = Directory.GetFiles(
                folderPath,
                "*.lol",
                SearchOption.AllDirectories
            );

            foreach (string file in files)
            {
                DecryptFile(file, password);
            }
        }
    }
}
