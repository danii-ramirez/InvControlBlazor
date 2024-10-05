using System.Security.Cryptography;
using System.Text;

namespace InvControl.Server.Helpers
{
    internal class Hashing
    {
        const int keySize = 64;
        const int iterations = 350000;
        readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        internal string HashPassword(string user, string password)
        {
            byte[] salt = Encoding.UTF8.GetBytes(user);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }

        internal bool VerifyPassword(string user, string hash, string password)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, Encoding.UTF8.GetBytes(user), iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
