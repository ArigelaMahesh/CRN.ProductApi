using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(
    int pageNumber,
    int pageSize);

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(CreateProductDto request);

        Task<bool> UpdateAsync(int id, UpdateProductDto request);

        Task<bool> DeleteAsync(int id);
    }
}
