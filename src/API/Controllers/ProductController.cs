using Application.DTOs;
using Application.DTOs.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
namespace API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]


public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(
    int pageNumber = 1,
    int pageSize = 10)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

        var result = await _productService
            .GetAllAsync(pageNumber, pageSize);

        return Ok(result);
    }

    // GET: api/Product/1
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return Ok(product);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
    [FromBody] CreateProductDto request)
    {
        var product = await _productService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateProductDto request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            return BadRequest(new
            {
                message = "Product name is required."
            });
        }

        var updated = await _productService.UpdateAsync(id, request);

        if (!updated)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return NoContent();
    }

    // DELETE: api/Product/1
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Product with id {id} was not found."
            });
        }

        return NoContent();
    }
}