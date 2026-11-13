using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.Options
{
    public class CryptoOptions
    {
        public const string SectionName = "Crypto";

        /// <summary>Khóa AES-256, chuỗi Base64 của 32 byte.</summary>
        public string EncryptionKey { get; set; } = string.Empty;

        /// <summary>Khóa HMAC-SHA256, chuỗi Base64 của 32 byte (khác khóa mã hóa).</summary>
        public string HmacKey { get; set; } = string.Empty;
    }
}
