using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp.AzureBlob.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly IContainerService _containerService;

        public ContainersController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            bool result = await _containerService.CreateContainer(containerName);
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            bool result = await _containerService.DeleteContainer(containerName);
            return Ok(result);
        }

        [HttpGet("GetAllContainers")]
        public async Task<IActionResult> GetAllContainers()
        {
            var result = await _containerService.GetAllContainer();
            return Ok(result);
        }

        [HttpGet("GetAllBlobs")]
        public async Task<IActionResult> GetAllCotnainersAndblobs()
        {
            var result = await _containerService.GetAllContainersAndBlobs();
            return Ok(result);
        }
    }
}
