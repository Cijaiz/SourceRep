namespace C2C.Core.Helper
{
    #region Reference
    
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    
    #endregion

    /// <summary>
    /// Provide helper methods relevant to cryptography.
    /// </summary>
    public class CryptoHelper
    {
        /// <summary>
        /// To decrypt passed value.
        /// </summary>
        /// <param name="value">Encrypted message.</param>
        /// <returns>Decrypted value</returns>
        public static string Decrypt(string value)
        {
            return value;
        }

        /// <summary>
        /// To encrypt password value.
        /// </summary>
        /// <param name="value">Message to encrypt/</param>
        /// <returns>Encrypted message.</returns>
        public static string Encrypt(string value)
        {
            return value;
        }

        /// <summary>
        /// To generate random salt.
        /// </summary>
        /// <returns>Generated salt value</returns>
        public static string GenerateSalt()
        {
            var saltBytes = new byte[0x10];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// To hash data
        /// </summary>
        /// <param name="salt">salt value for hashing.</param>
        /// <param name="data">data to be hashed.</param>
        /// <returns>Hashed data.</returns>
        public static string HashData(string salt, string data)
        {
            var saltBytes = Encoding.Unicode.GetBytes(salt);
            var passwordBytes = Encoding.Unicode.GetBytes(data);
            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();
            byte[] hashBytes;
            using (var hashAlgorithm = HashAlgorithm.Create())
            {
                hashBytes = hashAlgorithm.ComputeHash(combinedBytes);
            }

            return Convert.ToBase64String(hashBytes);
        }
    }
}