using System.Security.Cryptography;
using System.Text;

namespace AS_ASSN2_Rachel.Services
{
    public static class EncryptionHelper
    {
        private static readonly string EncryptionKey = "YourSecretKeyHere";

        public static string Decrypt(string encryptedText)
        {
            byte[] buffer = Convert.FromBase64String(encryptedText);
            using var aes = Aes.Create();
            var key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.Key = key.Take(32).ToArray();
            aes.IV = key.Take(16).ToArray();
            using var decryptor = aes.CreateDecryptor();
            var result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(result);
        }

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            var key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.Key = key.Take(32).ToArray();
            aes.IV = aes.Key.Take(16).ToArray();

            using var encryptor = aes.CreateEncryptor();
            byte[] buffer = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Convert.ToBase64String(encrypted);
        }
    }
}
