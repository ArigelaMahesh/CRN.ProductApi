using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Interfaces
{
    public interface IItemRepository
    {
        Task<List<Item>> GetByProductIdAsync(int productId);

        Task<Item?> GetByIdAsync(int id);

        Task<Item> AddAsync(Item item);

        Task<bool> DeleteAsync(int id);
    }
}
