using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Common.Shared
{
    public static class Hash
    {
        private const int MemorySize = 19456;
        private const int Iterations = 2;
        private const int Parallelism = 1;
        private const int SaltSize = 16;
        private const int HashSize = 32;


        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                // Convert hash byte array to a string
                StringBuilder hashBuilder = new StringBuilder();
                foreach (var b in hash)
                {
                    hashBuilder.Append(b.ToString("x2")); // Hexadecimal format
                }
                return hashBuilder.ToString();
            }
        }

        public static string Argon2(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            // Tạo salt ngẫu nhiên cho mỗi password
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = ComputeHash(
                password,
                salt,
                MemorySize,
                Iterations,
                Parallelism
            );

            // Lưu cả thuật toán, tham số, salt và hash
            return $"$argon2id$v=19$m={MemorySize},t={Iterations},p={Parallelism}" +
                   $"${Convert.ToBase64String(salt)}" +
                   $"${Convert.ToBase64String(hash)}";
        }

        public static bool VerifyArgon2(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passwordHash))
                return false;

            try
            {
                string[] parts = passwordHash.Split('$');

                if (parts.Length != 6 ||
                    parts[1] != "argon2id" ||
                    parts[2] != "v=19")
                    return false;

                // Đọc tham số đã lưu
                var parameters = parts[3].Split(',');

                if (parameters.Length != 3)
                    return false;

                int memory = int.Parse(parameters[0]["m=".Length..]);
                int iterations = int.Parse(parameters[1]["t=".Length..]);
                int parallelism = int.Parse(parameters[2]["p=".Length..]);

                // Giới hạn tham số để tránh hash lỗi gây tiêu tốn tài nguyên
                if (memory < 19456 || memory > 65536 ||
                    iterations < 2 || iterations > 5 ||
                    parallelism < 1 || parallelism > 4)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[4]);
                byte[] expectedHash = Convert.FromBase64String(parts[5]);

                if (salt.Length != SaltSize ||
                    expectedHash.Length != HashSize)
                    return false;

                byte[] actualHash = ComputeHash(
                    password,
                    salt,
                    memory,
                    iterations,
                    parallelism
                );

                // So sánh constant-time
                return CryptographicOperations.FixedTimeEquals(
                    actualHash,
                    expectedHash
                );
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static byte[] ComputeHash(
            string password,
            byte[] salt,
            int memory,
            int iterations,
            int parallelism)
        {
            using var argon2 = new Argon2id(
                Encoding.UTF8.GetBytes(password)
            );

            argon2.Salt = salt;
            argon2.MemorySize = memory;
            argon2.Iterations = iterations;
            argon2.DegreeOfParallelism = parallelism;

            return argon2.GetBytes(HashSize);
        }
    }
}
