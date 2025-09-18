namespace WriterApp.Endpoints;

public static class UploadEndpoints
{
    public static void MapUploadEndpoints(this WebApplication app)
    {
        app.MapPost("/api/upload", async (IFormFile file) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("No file selected");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return Results.BadRequest("Only image files are allowed");

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "images");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return Results.Ok(new { message = "File uploaded successfully" });
        });
    }
}
