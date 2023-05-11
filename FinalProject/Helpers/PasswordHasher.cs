using System;
using System.Text;
using XSystem.Security.Cryptography;

namespace LOAN_API.Helpers
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            var sha1 = new SHA1CryptoServiceProvider();

            byte[] password_bytes = Encoding.ASCII.GetBytes(password);
            byte[] encrypted_bytes = sha1.ComputeHash(password_bytes);
            return Convert.ToBase64String(encrypted_bytes);
        }
    }
}
