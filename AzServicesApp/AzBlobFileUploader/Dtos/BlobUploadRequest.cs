using Microsoft.AspNetCore.Mvc;

namespace AzBlobFileUploader.Dtos;

public class BlobUploadRequest
{
    public string ContainerName { get; set; }
    public IFormFile file { get; set; }
    [FromForm(Name = "metadata")] // This will collect all form fields starting with "metadata."
    public string MetadataJson { get; set; }
}
