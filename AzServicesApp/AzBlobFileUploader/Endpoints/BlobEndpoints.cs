using AzBlobFileUploader.Blob;
using AzBlobFileUploader.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AzBlobFileUploader.Endpoints;

public static class BlobEndpoints
{
    public static void MapBlobEndpoints(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api/blob");

        app.MapPost("/files", async ([FromForm] BlobUploadRequest request, IBlobService blobService, CancellationToken cancellationToken) =>
        {
            using Stream stream = request.file.OpenReadStream();

            Dictionary<string, string> metadata = null;
            if (!string.IsNullOrEmpty(request.MetadataJson))
            {
                metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(request.MetadataJson);
            }

            string blobUrl = await blobService.UploadAsync(request.ContainerName, stream, 
                                                           request.file.FileName,                                                            
                                                           request.file.ContentType,
                                                           metadata,
                                                           cancellationToken);
            
            return Results.Ok(blobUrl);

        }).WithTags("Files")
          .DisableAntiforgery();

        app.MapGet("/files/{containerName}/{id}", async (string containerName, Guid id, IBlobService blobService, CancellationToken cancellationToken) =>
        {
            FileResponse response = await blobService.DownloadAsync(containerName,id, cancellationToken);
            return Results.File(response.Stream, response.ContentType);
        }).WithTags("Files");

        app.MapDelete("/files/{containerName}/{id}", async (string containerName, Guid id, IBlobService blobService) =>
        {
            await blobService.DeleteAsync(containerName, id);

            return Results.NoContent();

        }).WithTags("Files");
    }
}
