using AzBlobStorageLocal.Services;

namespace AzBlobStorageLocal.Endpoints
{
    public static class BlobStorageEndpoints
    {
        public static void MapBlobStorageEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/files", async (IFormFile file, IBlobService blobService) =>
            {
                using Stream stream = file.OpenReadStream();
                Guid uploadedFileId = await blobService.UploadAsync(stream, file.ContentType);
                return Results.Ok(uploadedFileId);
            }).WithTags("Files")
              .DisableAntiforgery();

            app.MapGet("/files/{id}", async (Guid id, IBlobService blobService) =>
            {
                FileResponse response = await blobService.DownloadAsync(id);
                return Results.File(response.Stream, response.ContentType);
            }).WithTags("Files");

            app.MapDelete("/files/{id}", async (Guid id, IBlobService blobService) =>
            {
                await blobService.DeleteAsync(id);
                return Results.NoContent();
            }).WithTags("Files");
        }
    }
}
