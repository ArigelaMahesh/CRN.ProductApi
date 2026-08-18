using Application.DTOs;
using Application.DTOs.Interfaces;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<List<ItemDto>> GetByProductIdAsync(int productId)
    {
        var items = await _itemRepository.GetByProductIdAsync(productId);

        return items.Select(x => new ItemDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            Quantity = x.Quantity
        }).ToList();
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);

        if (item == null)
            return null;

        return new ItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity
        };
    }

    public async Task<ItemDto> AddAsync(CreateItemDto request)
    {
        var item = new Item
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        var created = await _itemRepository.AddAsync(item);

        return new ItemDto
        {
            Id = created.Id,
            ProductId = created.ProductId,
            Quantity = created.Quantity
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _itemRepository.DeleteAsync(id);
    }
}