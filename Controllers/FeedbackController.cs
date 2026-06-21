using Buza.Dtos;
using Buza.Services;
using Microsoft.AspNetCore.Mvc;

namespace Buza.Controllers;

[ApiController]
[Route("api/feedback")]
public class FeedbackController(IFeedbackService feedback, ISiteSettingsService settings) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FeedbackCreateDto dto)
    {
        var request = await feedback.CreateAsync(dto);
        var siteSettings = await settings.GetAsync();

        return Ok(new
        {
            message = siteSettings.FormSuccessMessage,
            request
        });
    }
}
