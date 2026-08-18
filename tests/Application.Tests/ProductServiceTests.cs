using Application.DTOs;
using Application.DTOs.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _service = new ProductService(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            ProductName = "Mobile Phone",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Mobile Phone", result.ProductName);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(999),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedProduct()
    {
        // Arrange
        var request = new CreateProductDto
        {
            ProductName = "Laptop"
        };

        var createdProduct = new Product
        {
            Id = 1,
            ProductName = "Laptop",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.ProductName);
        Assert.Equal("System", result.CreatedBy);

        _repositoryMock.Verify(
            x => x.AddAsync(It.Is<Product>(
                p => p.ProductName == "Laptop" &&
                     p.CreatedBy == "System")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        var request = new UpdateProductDto
        {
            ProductName = "Updated Product"
        };

        // Act
        var result = await _service.UpdateAsync(999, request);

        // Assert
        Assert.False(result);

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ProductExists_UpdatesProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            ProductName = "Old Product",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _repositoryMock
            .Setup(x => x.UpdateAsync(product))
            .ReturnsAsync(true);

        var request = new UpdateProductDto
        {
            ProductName = "Updated Product"
        };

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        Assert.True(result);
        Assert.Equal("Updated Product", product.ProductName);
        Assert.Equal("System", product.ModifiedBy);
        Assert.NotNull(product.ModifiedOn);

        _repositoryMock.Verify(
            x => x.UpdateAsync(product),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductExists_ReturnsTrue()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);

        _repositoryMock.Verify(
            x => x.DeleteAsync(1),
            Times.Once);
    }
}