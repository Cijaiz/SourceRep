using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;

namespace Octane.NotificationUtility
{
    public class PasswordHelper
    {
        public static string GeneratePasswordSalt()
        {
            var saltBytes = new byte[0x10];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        public static string GeneratePassword()
        {
            // Generates a random password of the specified length.
            return Membership.GeneratePassword(8, 1);
        }

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

