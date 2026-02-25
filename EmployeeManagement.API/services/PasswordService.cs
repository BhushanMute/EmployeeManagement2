using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace EmployeeManagement.API.services
{
    public class PasswordService : IPasswordService
    {
        public (string Hash, string Salt) HashPassword(string password)
        {
            // Generate salt
            byte[] saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            string salt = Convert.ToBase64String(saltBytes);

            // Hash password
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return (hash, salt);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            string computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hash == computedHash;
        }
    }
}
 
