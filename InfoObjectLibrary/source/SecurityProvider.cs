using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace InfoObject
{
    internal class SecurityProvider
    {
        // returns always a 32 length byte array from a given password
        private static byte[] keyFromPassword( string text)
        {
            int n = 32;
            if (text == null || text.Length == 0)
                return Array.Empty<byte>();
            if (text.Length < n ){
                for (int i = 0; i < n && text.Length<n ; i++)
                {
                    text += text[i];
                }
            }
            else if (text.Length > n)
            {
                text = text.Substring(0, n); 
            }
            return Encoding.ASCII.GetBytes(text);
        }
        private static byte[] IV = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static byte[] Encrypt(byte[] data, string password)
        {
            var key = keyFromPassword(password);
            return _EncryptBinary(data, key, IV);
        }
        public static byte[] Decrypt(byte[] data, string password)
        {
            var key = keyFromPassword(password);
            return _DecryptBinary(data, key, IV);
        }

        private static byte[] _EncryptBinary(byte[] data, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.FeedbackSize = 128;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        private static byte[] _DecryptBinary(byte[] data, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            byte[] decrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.FeedbackSize = 128;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(data, 0, data.Length);
                        csDecrypt.FlushFinalBlock();
                        decrypted = msDecrypt.ToArray();
                    }
                }
            }
            // Return the decrypted bytes from the memory stream.
            return decrypted;
        }

        public static bool IsPasswordCorrect(string passwordToTest, string hashedpassword, string salt)
        {
            string pass = CreateSHA512(passwordToTest + salt);
            if (hashedpassword == pass)
                return true;
            return false;
        }
        private static string CreateSHA512(string strData)
        {
            var message = Encoding.UTF8.GetBytes(strData);
            using (var alg = SHA512.Create())
            {
                string hex = "";
                var hashValue = alg.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;
            }
        }
        public static string GetSalt()
        {
            return Guid.NewGuid().ToString();
        }
        public static string GetHashedPawssword(string password, string salt)
        {
            return CreateSHA512(password + salt);
        }





    }
}
