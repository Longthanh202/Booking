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
    }
}
