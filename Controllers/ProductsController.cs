using Buza.Services;
using Microsoft.AspNetCore.Mvc;

namespace Buza.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await products.GetPublicAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => Ok(await products.GetPublicByIdAsync(id));
}
