using Buza.Dtos;
using Buza.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buza.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/products")]
public class AdminProductsController(IProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await products.GetAllForAdminAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(await products.GetAdminByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductUpsertDto dto)
    {
        var product = await products.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpsertDto dto) => Ok(await products.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await products.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/images")]
    [RequestSizeLimit(5_242_880)]
    public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile? file)
    {
        if (file is null)
        {
            return BadRequest(new { error = "Файл не передан." });
        }

        return Ok(await products.AddImageAsync(id, file));
    }

    [HttpDelete("{productId:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int productId, int imageId)
    {
        await products.DeleteImageAsync(productId, imageId);
        return NoContent();
    }
}
