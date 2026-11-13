using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Cryptos
{
    public interface ICryptoService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        byte[] ComputeHash(string value);     // HMAC-SHA256 với khóa bí mật, trả về 32 byte
    }
}
