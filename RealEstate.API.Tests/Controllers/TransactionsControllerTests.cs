using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.API.Controllers;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;
using RealEstate.API.Models.Pagination;
using RealEstate.API.Services;

namespace RealEstate.API.Tests.Controllers;

public class TransactionsControllerTests
{
    private RealEstateDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RealEstateDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new RealEstateDbContext(options);
    }

    [Fact]
    public async Task GetTransactions_WithPagination_ReturnsPagedResult()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        
        // Setup test data
        var client = new Client { Name = "Client", Email = "client@test.com", PhoneNumber = "111" };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var property = new Property { Address = "Address", Price = 100000, Type = "Sale", OwnerId = client.Id };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var controller = new TransactionsController(service);

        // Add transactions
        context.Transactions.AddRange(
            new Transaction { PropertyId = property.Id, ClientId = client.Id, Amount = 1000, TransactionType = "Sale", Date = DateTime.UtcNow },
            new Transaction { PropertyId = property.Id, ClientId = client.Id, Amount = 2000, TransactionType = "Rent", Date = DateTime.UtcNow },
            new Transaction { PropertyId = property.Id, ClientId = client.Id, Amount = 3000, TransactionType = "Sale", Date = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 2 };

        // Act
        var result = await controller.GetTransactions(paginationParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var pagedResult = Assert.IsType<PagedResult<TransactionDto>>(okResult.Value);
        Assert.Equal(2, pagedResult.Items.Count());
        Assert.Equal(3, pagedResult.TotalCount);
    }

    [Fact]
    public async Task GetTransaction_ExistingId_ReturnsTransaction()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var client = new Client { Name = "Client", Email = "client@test.com", PhoneNumber = "111" };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var property = new Property { Address = "Address", Price = 100000, Type = "Sale", OwnerId = client.Id };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var controller = new TransactionsController(service);

        var transaction = new Transaction { PropertyId = property.Id, ClientId = client.Id, Amount = 5000, TransactionType = "Sale", Date = DateTime.UtcNow };
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetTransaction(transaction.Id);

        // Assert
        Assert.NotNull(result.Value);
        var transactionDto = result.Value;
        Assert.Equal(transaction.Id, transactionDto.Id);
    }

    [Fact]
    public async Task GetTransaction_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new TransactionService(context);
        var controller = new TransactionsController(service);

        // Act
        var result = await controller.GetTransaction(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateTransaction_ValidData_ReturnsCreatedTransaction()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var client = new Client { Name = "Client", Email = "client@test.com", PhoneNumber = "111" };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var property = new Property { Address = "Address", Price = 100000, Type = "Sale", OwnerId = client.Id };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var controller = new TransactionsController(service);

        var createDto = new CreateTransactionDto
        {
            PropertyId = property.Id,
            ClientId = client.Id,
            Amount = 10000,
            TransactionType = "Sale"
        };

        // Act
        var result = await controller.CreateTransaction(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var transactionDto = Assert.IsType<TransactionDto>(createdResult.Value);
        Assert.Equal(10000, transactionDto.Amount);
    }

    [Fact]
    public async Task CreateTransaction_WithPaginationOnList_WorksCorrectly()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var client = new Client { Name = "Client", Email = "client@test.com", PhoneNumber = "111" };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var property = new Property { Address = "Address", Price = 100000, Type = "Sale", OwnerId = client.Id };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var controller = new TransactionsController(service);

        // Create multiple transactions
        for (int i = 0; i < 5; i++)
        {
            var createDto = new CreateTransactionDto
            {
                PropertyId = property.Id,
                ClientId = client.Id,
                Amount = 1000 * (i + 1),
                TransactionType = "Sale"
            };
            await controller.CreateTransaction(createDto);
        }

        var paginationParams = new PaginationParams { PageNumber = 2, PageSize = 2 };

        // Act
        var result = await controller.GetTransactions(paginationParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var pagedResult = Assert.IsType<PagedResult<TransactionDto>>(okResult.Value);
        Assert.Equal(2, pagedResult.Items.Count());
        Assert.Equal(5, pagedResult.TotalCount);
        Assert.Equal(3, pagedResult.TotalPages);
        Assert.True(pagedResult.HasPrevious);
        Assert.True(pagedResult.HasNext);
    }
}
