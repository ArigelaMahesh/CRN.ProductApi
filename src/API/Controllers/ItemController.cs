using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }
    [Authorize]
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var items = await _itemService.GetByProductIdAsync(productId);

        return Ok(items);
    }
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _itemService.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound(new
            {
                message = $"Item with id {id} was not found."
            });
        }

        return Ok(item);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add(CreateItemDto request)
    {
        if (request.Quantity <= 0)
        {
            return BadRequest(new
            {
                message = "Quantity must be greater than zero."
            });
        }

        var item = await _itemService.AddAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.Id },
            item);
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _itemService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Item with id {id} was not found."
            });
        }

        return NoContent();
    }
}