using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using System.Reflection.Metadata.Ecma335;

namespace RealtimeUsecaseApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public ReviewsController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitReview([FromBody] ReviewRequest review)
    {
        //Feature Flag: Enable User Reviews
        if(!await _featureManager.IsEnabledAsync(FeatureFlags.EnableUserReviews))
        {
            return BadRequest("User Reviews are currently disabled");
        }

        //Process the review submission (mocked)
        var response = new { Message = "Review Submitted Successfully"};

        //Feature Flag: EnableAISummary
        if (await _featureManager.IsEnabledAsync("EnableAISummary"))
        {
            response = new
            {
                Message = "Review submitted successfully. {AISummary here}",                
            };
        }

        return Ok(response); 
    }

    [HttpGet("badge/{userId}")]
    public async Task<IActionResult> GetBadgeStatus(string userId)
    {
        if (await _featureManager.IsEnabledAsync(FeatureFlags.EnableVerifiedBadge))
        {
            return Ok(new { Badge = "Verified Purchase" });
        }

        return Ok( new { Badge = "Standard User" });
    }

}




public class ReviewRequest
{
    public string ProductId { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
}
