using ApplicationLayer.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace InfrastructureLayer.Repositories
{
    /// <summary>
    /// Provides functionality for creating password hashes and verifying passwords.
    /// Uses HMACSHA512 for cryptographic operations.
    /// </summary>
    public class PasswordHandler : IPasswordHandler
    {

        /// <summary>
        /// Generates a cryptographic salt and hashes the given password using HMACSHA512.
        /// </summary>
        /// <param name="password">The plain-text password to be hashed.</param>
        /// <param name="passwordHash">The resulting password hash as a byte array.</param>
        /// <param name="passwordSalt">The generated cryptographic salt (HMAC key) used for hashing.</param>
        public void CreateSaltAndHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            passwordSalt = hmac.Key;
        }

        /// <summary>
        /// Verifies whether the provided password matches the stored hash using the given salt.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="passwordHash">The stored password hash for comparison.</param>
        /// <param name="passwordSalt">The salt originally used to hash the password (HMAC key).</param>
        /// <returns>True if the password is valid; otherwise, false.</returns>
        public bool VerifyPasswordHash(string password, in byte[] passwordHash, in byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return AreEqualArrays(computedHash, passwordHash);
        }

        /// <summary>
        /// Compares two byte arrays in a time-constant manner to prevent timing attacks.
        /// </summary>
        /// <param name="computedHash">The hash computed from the input password.</param>
        /// <param name="passwordHash">The stored password hash to compare against.</param>
        /// <returns>True if both arrays are equal; otherwise, false.</returns>
        private bool AreEqualArrays(in byte[] computedHash, in byte[] passwordHash)
        {
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                    return false;
            }
            return true;
        }
    }
}
