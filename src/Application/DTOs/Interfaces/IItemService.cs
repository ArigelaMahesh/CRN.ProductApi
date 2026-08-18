using Application.DTOs;

namespace Application.Interfaces;

public interface IItemService
{
    Task<List<ItemDto>> GetByProductIdAsync(int productId);

    Task<ItemDto?> GetByIdAsync(int id);

    Task<ItemDto> AddAsync(CreateItemDto request);

    Task<bool> DeleteAsync(int id);
}