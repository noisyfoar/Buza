using Buza.Dtos;
using Buza.Services;
using Microsoft.AspNetCore.Mvc;

namespace Buza.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto) => Ok(await auth.LoginAsync(dto));
}
