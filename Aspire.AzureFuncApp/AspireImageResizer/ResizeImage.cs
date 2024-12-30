using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace AspireImageResizer
{
    public class ResizeImage
    {
        private readonly ILogger<ResizeImage> _logger;

        public ResizeImage(ILogger<ResizeImage> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ResizeImage))]
        [BlobOutput("resized-images/{name}",Connection = "blobs")]
        public byte[] Run([BlobTrigger("uploads/{name}", Connection = "blobs")] byte[] image, string name)
        {
            try
            {
                _logger.LogInformation("Generating thumbnail for image {Name}", name);
                return GetResizedImageStream(name, image, SKEncodedImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Error generating thumbnail image:{Name}. Exception {Message}",
                    name, ex.Message);
                throw;
            }
        }

        private byte[] GetResizedImageStream(string name, byte[] image, SKEncodedImageFormat jpeg)
        {
            // Resize the image
            using var inputStream = new MemoryStream(image);
            using var inputBitmap = SKBitmap.Decode(inputStream);
            using var outputStream = new MemoryStream();
            using var outputBitmap = inputBitmap.Resize(new SKImageInfo(100, 100), SKFilterQuality.Medium);
            outputBitmap.Encode(outputStream, jpeg, 100);
            return outputStream.ToArray();
        } 
    }
}
