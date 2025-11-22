using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.API.Controllers;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;
using RealEstate.API.Models.Pagination;
using RealEstate.API.Services;

namespace RealEstate.API.Tests.Controllers;

public class ClientsControllerTests
{
    private RealEstateDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RealEstateDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new RealEstateDbContext(options);
    }

    [Fact]
    public async Task GetClients_WithPagination_ReturnsPagedResult()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ClientService(context);
        var controller = new ClientsController(service);

        // Add test data
        context.Clients.AddRange(
            new Client { Name = "Client 1", Email = "client1@test.com", PhoneNumber = "111" },
            new Client { Name = "Client 2", Email = "client2@test.com", PhoneNumber = "222" },
            new Client { Name = "Client 3", Email = "client3@test.com", PhoneNumber = "333" }
        );
        await context.SaveChangesAsync();

        var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 2 };

        // Act
        var result = await controller.GetClients(paginationParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var pagedResult = Assert.IsType<PagedResult<ClientDto>>(okResult.Value);
        Assert.Equal(2, pagedResult.Items.Count());
        Assert.Equal(3, pagedResult.TotalCount);
        Assert.Equal(2, pagedResult.TotalPages);
        Assert.True(pagedResult.HasNext);
        Assert.False(pagedResult.HasPrevious);
    }

    [Fact]
    public async Task GetClient_ExistingId_ReturnsClient()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ClientService(context);
        var controller = new ClientsController(service);

        var client = new Client { Name = "Test Client", Email = "test@test.com", PhoneNumber = "123" };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetClient(client.Id);

        // Assert
        Assert.NotNull(result.Value);
        var clientDto = result.Value;
        Assert.Equal(client.Id, clientDto.Id);
        Assert.Equal("Test Client", clientDto.Name);
    }


    [Fact]
    public async Task GetClient_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ClientService(context);
        var controller = new ClientsController(service);

        // Act
        var result = await controller.GetClient(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateClient_ValidData_ReturnsCreatedClient()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ClientService(context);
        var controller = new ClientsController(service);

        var createDto = new CreateClientDto
        {
            Name = "New Client",
            Email = "new@test.com",
            PhoneNumber = "555"
        };

        // Act
        var result = await controller.CreateClient(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var clientDto = Assert.IsType<ClientDto>(createdResult.Value);
        Assert.Equal("New Client", clientDto.Name);
        Assert.Equal("new@test.com", clientDto.Email);
    }

    [Fact]
    public async Task UpdateClient_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ClientService(context);
        var controller = new ClientsController(service);

        var client = new Client { Name = "Original", Email = "original@test.com", PhoneNumber = "111" };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var updateDto = new UpdateClientDto
        {
            Name = "Updated",
            Email = "updated@test.com",
            PhoneNumber = "222"
        };

        // Act
        var result = await controller.UpdateClient(client.Id, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteClient_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new ClientService(context);
        var controller = new ClientsController(service);

        var client = new Client { Name = "To Delete", Email = "delete@test.com", PhoneNumber = "999" };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.DeleteClient(client.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Clients);
    }
}
