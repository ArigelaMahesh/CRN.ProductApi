
using Application.DTOs;
using Application.DTOs.Interfaces;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.Tests;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _repositoryMock;
    private readonly ItemService _service;

    public ItemServiceTests()
    {
        _repositoryMock = new Mock<IItemRepository>();

        _service = new ItemService(
            _repositoryMock.Object);
    }

    [Fact]
    public async Task GetByProductIdAsync_ProductHasItems_ReturnsItems()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item
            {
                Id = 1,
                ProductId = 10,
                Quantity = 5
            },
            new Item
            {
                Id = 2,
                ProductId = 10,
                Quantity = 10
            }
        };

        _repositoryMock
            .Setup(x => x.GetByProductIdAsync(10))
            .ReturnsAsync(items);

        // Act
        var result = await _service.GetByProductIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Id);
        Assert.Equal(10, result[0].ProductId);
        Assert.Equal(5, result[0].Quantity);

        Assert.Equal(2, result[1].Id);
        Assert.Equal(10, result[1].ProductId);
        Assert.Equal(10, result[1].Quantity);

        _repositoryMock.Verify(
            x => x.GetByProductIdAsync(10),
            Times.Once);
    }

    [Fact]
    public async Task GetByProductIdAsync_ProductHasNoItems_ReturnsEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetByProductIdAsync(999))
            .ReturnsAsync(new List<Item>());

        // Act
        var result = await _service.GetByProductIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _repositoryMock.Verify(
            x => x.GetByProductIdAsync(999),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ItemExists_ReturnsItem()
    {
        // Arrange
        var item = new Item
        {
            Id = 1,
            ProductId = 10,
            Quantity = 25
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(item);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.ProductId);
        Assert.Equal(25, result.Quantity);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ItemDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(999),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_ReturnsCreatedItem()
    {
        // Arrange
        var request = new CreateItemDto
        {
            ProductId = 10,
            Quantity = 50
        };

        var createdItem = new Item
        {
            Id = 1,
            ProductId = 10,
            Quantity = 50
        };

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Item>()))
            .ReturnsAsync(createdItem);

        // Act
        var result = await _service.AddAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.ProductId);
        Assert.Equal(50, result.Quantity);

        _repositoryMock.Verify(
            x => x.AddAsync(It.Is<Item>(
                i => i.ProductId == 10 &&
                     i.Quantity == 50)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ItemExists_ReturnsTrue()
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

    [Fact]
    public async Task DeleteAsync_ItemDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        Assert.False(result);

        _repositoryMock.Verify(
            x => x.DeleteAsync(999),
            Times.Once);
    }
}

