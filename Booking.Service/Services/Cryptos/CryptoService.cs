using Booking.Core.Model.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Cryptos
{
    public class CryptoService : ICryptoService
    {
        private const string Prefix = "v1:";
        private const int NonceSize = 12;   // AES-GCM chuẩn: 96 bit
        private const int TagSize = 16;     // 128 bit

        private readonly byte[] _encKey;
        private readonly byte[] _hmacKey;

        public CryptoService(IOptions<CryptoOptions> options)
        {
            _encKey = ParseKey(options.Value.EncryptionKey, nameof(CryptoOptions.EncryptionKey));
            _hmacKey = ParseKey(options.Value.HmacKey, nameof(CryptoOptions.HmacKey));

            if (_encKey.AsSpan().SequenceEqual(_hmacKey))
                throw new InvalidOperationException("EncryptionKey và HmacKey phải khác nhau.");
        }

        public string Encrypt(string plainText)
        {
            ArgumentNullException.ThrowIfNull(plainText);

            var plain = Encoding.UTF8.GetBytes(plainText);
            var nonce = RandomNumberGenerator.GetBytes(NonceSize);   // nonce mới cho mỗi lần mã hóa
            var cipher = new byte[plain.Length];
            var tag = new byte[TagSize];

            using (AesGcm aes = new AesGcm(_encKey))
            {
                aes.Encrypt(nonce, plain, cipher, tag);
            }

            // nonce | tag | cipher
            var payload = new byte[NonceSize + TagSize + cipher.Length];
            Buffer.BlockCopy(nonce, 0, payload, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, payload, NonceSize, TagSize);
            Buffer.BlockCopy(cipher, 0, payload, NonceSize + TagSize, cipher.Length);

            CryptographicOperations.ZeroMemory(plain);
            return Prefix + Convert.ToBase64String(payload);
        }

        public string Decrypt(string cipherText)
        {
            ArgumentNullException.ThrowIfNull(cipherText);

            if (!cipherText.StartsWith(Prefix, StringComparison.Ordinal))
                throw new CryptographicException("Định dạng bản mã không hợp lệ hoặc không đúng phiên bản.");

            var payload = Convert.FromBase64String(cipherText[Prefix.Length..]);
            if (payload.Length < NonceSize + TagSize)
                throw new CryptographicException("Bản mã không hợp lệ.");

            var nonce = payload.AsSpan(0, NonceSize);
            var tag = payload.AsSpan(NonceSize, TagSize);
            var cipher = payload.AsSpan(NonceSize + TagSize);
            var plain = new byte[cipher.Length];

            using (AesGcm aes = new AesGcm(_encKey))
            {
                aes.Encrypt(nonce, plain, cipher, tag);
            }

            var result = Encoding.UTF8.GetString(plain);
            CryptographicOperations.ZeroMemory(plain);
            return result;
        }

        public byte[] ComputeHash(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return HMACSHA256.HashData(_hmacKey, Encoding.UTF8.GetBytes(value));   // 32 byte
        }

        private static byte[] ParseKey(string base64, string name)
        {
            if (string.IsNullOrWhiteSpace(base64))
                throw new InvalidOperationException($"Chưa cấu hình {name}.");

            byte[] key;
            try { key = Convert.FromBase64String(base64); }
            catch (FormatException) { throw new InvalidOperationException($"{name} phải là chuỗi Base64."); }

            if (key.Length != 32)
                throw new InvalidOperationException($"{name} phải đúng 32 byte (hiện {key.Length}).");

            return key;
        }
    }
}
