using Buza.Dtos;
using Buza.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buza.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/settings")]
public class AdminSettingsController(ISiteSettingsService settings) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSiteSettingsDto dto) => Ok(await settings.UpdateAsync(dto));
}
