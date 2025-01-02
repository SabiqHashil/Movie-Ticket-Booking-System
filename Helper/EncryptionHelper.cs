using System.Security.Cryptography;
using System.Text;

namespace MovieTicketBookingSystem.Helper
{
    public static class EncryptionHelper
    {
        private static readonly string Key = "abcdefghijklmnopqrstuvwx12345678";
        private static readonly string IV = "1234567890abcdef";

        public static string EncryptPassword(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }
    }
}
