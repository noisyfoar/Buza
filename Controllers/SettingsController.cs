using Buza.Services;
using Microsoft.AspNetCore.Mvc;

namespace Buza.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController(ISiteSettingsService settings) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await settings.GetAsync());
}
