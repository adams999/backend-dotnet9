using Microsoft.EntityFrameworkCore;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;
using RealEstate.API.Services;

namespace RealEstate.API.Tests.Services;

public class PropertyServiceTests : IDisposable
{
    private readonly RealEstateDbContext _context;
    private readonly PropertyService _service;

    public PropertyServiceTests()
    {
        var options = new DbContextOptionsBuilder<RealEstateDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RealEstateDbContext(options);
        _service = new PropertyService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllPropertiesAsync_ReturnsAllProperties()
    {
        // Arrange
        _context.Properties.AddRange(
            new Property { Id = 1, Address = "123 Main St", Price = 250000, Type = "Sale", OwnerId = 1 },
            new Property { Id = 2, Address = "456 Oak Ave", Price = 350000, Type = "Rent", OwnerId = 2 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllPropertiesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPropertyByIdAsync_ExistingProperty_ReturnsProperty()
    {
        // Arrange
        var property = new Property { Id = 1, Address = "123 Main St", Price = 250000, Type = "Sale", OwnerId = 1 };
        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPropertyByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("123 Main St", result.Address);
    }

    [Fact]
    public async Task GetPropertyByIdAsync_NonExistingProperty_ReturnsNull()
    {
        // Act
        var result = await _service.GetPropertyByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePropertyAsync_ValidProperty_ReturnsCreatedProperty()
    {
        // Arrange
        var createDto = new CreatePropertyDto
        {
            Address = "789 New St",
            Price = 450000,
            Type = "Sale",
            Description = "New property",
            OwnerId = 1
        };

        // Act
        var result = await _service.CreatePropertyAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("789 New St", result.Address);
        Assert.Equal(450000, result.Price);
        
        // Verify it was saved to database
        var savedProperty = await _context.Properties.FindAsync(result.Id);
        Assert.NotNull(savedProperty);
    }

    [Fact]
    public async Task DeletePropertyAsync_ExistingProperty_ReturnsTrue()
    {
        // Arrange
        var property = new Property { Id = 1, Address = "123 Main St", Price = 250000, Type = "Sale", OwnerId = 1 };
        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeletePropertyAsync(1);

        // Assert
        Assert.True(result);
        
        // Verify it was deleted
        var deletedProperty = await _context.Properties.FindAsync(1);
        Assert.Null(deletedProperty);
    }

    [Fact]
    public async Task DeletePropertyAsync_NonExistingProperty_ReturnsFalse()
    {
        // Act
        var result = await _service.DeletePropertyAsync(999);

        // Assert
        Assert.False(result);
    }
}
