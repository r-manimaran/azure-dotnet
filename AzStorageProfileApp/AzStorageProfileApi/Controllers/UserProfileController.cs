using AzStorageProfileApi.Dtos;
using AzStorageProfileApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzStorageProfileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileService userProfileService, 
                                     IBlobStorageService blobStorageService,
                                     ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserProfile([FromForm] UserProfileCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Error = "Invalid input data",
                        Details = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var response = await _userProfileService.CreateUserProfile(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument in CreateUserProfile");
                return BadRequest(new { Error = "Invalid input format", Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateUserProfile");
                return StatusCode(500, new { Error = "An unexpected error occurred" });
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserProfile(string email)
        {
            var response = await _userProfileService.GetUserProfile(email);
            return Ok(response);
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetImageMetadata(string imageUrl)
        {
            try
            {
                var metadata = await _blobStorageService.GetBlobMetadataAsync(
                    imageUrl,
                    "profileimages");

                return Ok(metadata);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving metadata");
                return StatusCode(500, "Error retrieving metadaa");
            }
        }

        [HttpPut("metadata")]
        public async Task<IActionResult> UpdateImageMetadata(string imageUrl, [FromBody] Dictionary<string,string> metadata)
        {
            try
            {
                await _blobStorageService.UpdateBlobMetadataAsync(imageUrl,
                    "profilepictures",
                    metadata);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating metadata");
                return StatusCode(500, "Error updating metadata");
            }
        }
    }
}
