using Application.DTOs;
using Application.DTOs.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository repository,
            ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(
    int pageNumber,
    int pageSize)
        {
            var result = await _repository
                .GetAllAsync(pageNumber, pageSize);

            var products = result.Products;

            var totalCount = result.TotalCount;

            var productDtos = products.Select(x => new ProductDto
            {
                Id = x.Id,
                ProductName = x.ProductName,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                ModifiedBy = x.ModifiedBy,
                ModifiedOn = x.ModifiedOn
            }).ToList();

            return new PagedResult<ProductDto>
            {
                Items = productDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize)
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto request)
        {
            var product = new Product
            {
                ProductName = request.ProductName,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow
            };

            var createdProduct = await _repository.AddAsync(product);

            return MapToDto(createdProduct);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateProductDto request)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            product.ProductName = request.ProductName;
            product.ModifiedBy = "System";
            product.ModifiedOn = DateTime.UtcNow;

            return await _repository.UpdateAsync(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CreatedBy = product.CreatedBy,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedOn = product.ModifiedOn
            };
        }
    }
}
