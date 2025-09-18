using Microsoft.AspNetCore.Components.Forms;

namespace WriterApp.Services;

public class UploadService
{
    public async Task<(bool Success, string Message)> UploadImageAsync(IBrowserFile file)
    {
        try
        {
            if (file == null || file.Size == 0)
                return (false, "No file selected");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(file.Name).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return (false, "Only image files are allowed");

            if (!file.ContentType.StartsWith("image/"))
                return (false, "Invalid file type");

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "images");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, fileName);

            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            return (true, "Image uploaded successfully!");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }
}

