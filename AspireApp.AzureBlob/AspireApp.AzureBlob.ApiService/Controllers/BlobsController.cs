using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp.AzureBlob.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;

        public BlobsController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            await _blobStorageService.UploadBlobAsync("mydatastore", file.FileName, stream);
            return Ok();
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download(string blobName)
        {
            var stream = await _blobStorageService.DownloadBlobAsync("mydatastore", blobName);

            return File(stream, "application/octet-stream", blobName);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(string blobName)
        {
            await _blobStorageService.DeleteBlobAsync("mydatastore", blobName);
            return Ok();
        }

        [HttpGet("BlobsWithUri")]
        public async Task<IActionResult> GetBlobsWithUri(string containerName)
        {
            var result  = await _blobStorageService.GetAllBlobsWithUri(containerName);           
            return Ok(result);
        }

        [HttpGet("GetBlob")]
        public async Task<IActionResult> GetBlob(string containerName, string blobName)
        {
            var result = await _blobStorageService.GetBlob(containerName, blobName);
            return Ok(result);
        }

    }
}
