using Buza.Dtos;
using Buza.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buza.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/feedback")]
public class AdminFeedbackController(IFeedbackService feedback) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await feedback.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(await feedback.GetByIdAsync(id));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] FeedbackStatusUpdateDto dto) =>
        Ok(await feedback.UpdateStatusAsync(id, dto.Status));
}
