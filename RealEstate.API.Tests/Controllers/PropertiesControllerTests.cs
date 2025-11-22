using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.API.Controllers;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;
using RealEstate.API.Models.Pagination;
using RealEstate.API.Services;

namespace RealEstate.API.Tests.Controllers;

public class PropertiesControllerTests
{
    private RealEstateDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RealEstateDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new RealEstateDbContext(options);
    }

    [Fact]
    public async Task GetProperties_WithPagination_ReturnsPagedResult()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        
        // Add owner first
        var owner = new Client { Name = "Owner", Email = "owner@test.com", PhoneNumber = "111" };
        context.Clients.Add(owner);
        await context.SaveChangesAsync();

        var service = new PropertyService(context);
        var controller = new PropertiesController(service);

        // Add test data
        context.Properties.AddRange(
            new Property { Address = "Address 1", Price = 100000, Type = "Sale", OwnerId = owner.Id },
            new Property { Address = "Address 2", Price = 200000, Type = "Rent", OwnerId = owner.Id },
            new Property { Address = "Address 3", Price = 300000, Type = "Sale", OwnerId = owner.Id }
        );
        await context.SaveChangesAsync();

        var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 2 };

        // Act
        var result = await controller.GetProperties(paginationParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var pagedResult = Assert.IsType<PagedResult<PropertyDto>>(okResult.Value);
        Assert.Equal(2, pagedResult.Items.Count());
        Assert.Equal(3, pagedResult.TotalCount);
    }

    [Fact]
    public async Task GetProperty_ExistingId_ReturnsProperty()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var owner = new Client { Name = "Owner", Email = "owner@test.com", PhoneNumber = "111" };
        context.Clients.Add(owner);
        await context.SaveChangesAsync();

        var service = new PropertyService(context);
        var controller = new PropertiesController(service);

        var property = new Property { Address = "Test Address", Price = 150000, Type = "Sale", OwnerId = owner.Id };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetProperty(property.Id);

        // Assert
        Assert.NotNull(result.Value);
        var propertyDto = result.Value;
        Assert.Equal(property.Id, propertyDto.Id);
    }

    [Fact]
    public async Task GetProperty_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new PropertyService(context);
        var controller = new PropertiesController(service);

        // Act
        var result = await controller.GetProperty(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProperty_ValidData_ReturnsCreatedProperty()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var owner = new Client { Name = "Owner", Email = "owner@test.com", PhoneNumber = "111" };
        context.Clients.Add(owner);
        await context.SaveChangesAsync();

        var service = new PropertyService(context);
        var controller = new PropertiesController(service);

        var createDto = new CreatePropertyDto
        {
            Address = "New Address",
            Price = 250000,
            Type = "Sale",
            Description = "Test",
            OwnerId = owner.Id
        };

        // Act
        var result = await controller.CreateProperty(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var propertyDto = Assert.IsType<PropertyDto>(createdResult.Value);
        Assert.Equal("New Address", propertyDto.Address);
    }

    [Fact]
    public async Task UpdateProperty_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var owner = new Client { Name = "Owner", Email = "owner@test.com", PhoneNumber = "111" };
        context.Clients.Add(owner);
        await context.SaveChangesAsync();

        var service = new PropertyService(context);
        var controller = new PropertiesController(service);

        var property = new Property { Address = "Original", Price = 100000, Type = "Sale", OwnerId = owner.Id };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var updateDto = new UpdatePropertyDto
        {
            Address = "Updated",
            Price = 200000,
            Type = "Rent",
            Description = "Updated"
        };

        // Act
        var result = await controller.UpdateProperty(property.Id, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProperty_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var owner = new Client { Name = "Owner", Email = "owner@test.com", PhoneNumber = "111" };
        context.Clients.Add(owner);
        await context.SaveChangesAsync();

        var service = new PropertyService(context);
        var controller = new PropertiesController(service);

        var property = new Property { Address = "To Delete", Price = 100000, Type = "Sale", OwnerId = owner.Id };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.DeleteProperty(property.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
