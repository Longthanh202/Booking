using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Booking.Common.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Booking.Service.Services.Cloudinarys
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length <= 0) return null;

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "hotel-images", // thư mục trên cloud
                PublicId = Guid.NewGuid().ToString()
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<bool> DeleteImageAsync(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var imageUri)) return false;
            var segments = imageUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var folderIndex = Array.IndexOf(segments, "hotel-images");
            if (folderIndex < 0 || folderIndex == segments.Length - 1) return false;

            var publicId = string.Join("/", segments.Skip(folderIndex));
            var extensionIndex = publicId.LastIndexOf('.');
            if (extensionIndex > publicId.LastIndexOf('/'))
            {
                publicId = publicId[..extensionIndex];
            }

            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            return result.Result == "ok" || result.Result == "not found";
        }
    }
}
