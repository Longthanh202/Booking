

using Microsoft.AspNetCore.Http;

namespace Booking.Common.Shared
{
    public static class ImageUploadHelper
    {
        private static readonly HashSet<string> _allowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".webp"
            };
        private const long _maxFileSizeBytes = 10 * 1024 * 1024;

        /// <summary>
        /// Lưu file ảnh upload từ client vào thư mục chỉ định, trả về tên file đã lưu.
        /// </summary>
        /// <param name="file">File ảnh nhận từ client (IFormFile)</param>
        /// <param name="uploadFolder">Đường dẫn thư mục lưu ảnh</param>
        public static async Task<string> SaveImageAsync(IFormFile file, string uploadFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ hoặc rỗng.");

            if (file.Length > _maxFileSizeBytes)
                throw new ArgumentException("Kích thước file vượt quá giới hạn cho phép (10MB).");

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                throw new ArgumentException("Định dạng file không được hỗ trợ.");

            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(uploadFolder);

            // Đặt tên file theo timestamp + GUID rút gọn để tránh trùng khi nhiều request cùng lúc
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            var uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
            var fileName = $"{timestamp}_{uniqueSuffix}{extension}";

            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
