using System.Security.Cryptography;

namespace SoundPlayerAPI.Service
{
    public class PasswordHasher
    {
        private const int HashSize = 32; 
        private const int Iteration = 50000; 

        public static string HashPassword(string password)
        {
            using (var rfc2998 = new Rfc2898DeriveBytes(password, new byte[0], Iteration, HashAlgorithmName.SHA256))
            {
                byte[] hash = rfc2998.GetBytes(HashSize);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(password, new byte[0], Iteration, HashAlgorithmName.SHA256))
            {
                byte[] hash = rfc2898.GetBytes(HashSize);
                return Convert.ToBase64String(hash) == storedHash;
            }
        }
    }

}
