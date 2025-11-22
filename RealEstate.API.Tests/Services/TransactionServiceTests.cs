using Microsoft.EntityFrameworkCore;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;
using RealEstate.API.Services;

namespace RealEstate.API.Tests.Services;

public class TransactionServiceTests : IDisposable
{
    private readonly RealEstateDbContext _context;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        var options = new DbContextOptionsBuilder<RealEstateDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RealEstateDbContext(options);
        _service = new TransactionService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ReturnsAllTransactions()
    {
        // Arrange
        var property1 = new Property { Id = 1, Address = "123 Main St", Price = 250000, Type = "Sale", OwnerId = 1 };
        var property2 = new Property { Id = 2, Address = "456 Oak Ave", Price = 350000, Type = "Rent", OwnerId = 2 };
        var client1 = new Client { Id = 1, Name = "John Doe", Email = "john@test.com" };
        var client2 = new Client { Id = 2, Name = "Jane Smith", Email = "jane@test.com" };

        _context.Properties.AddRange(property1, property2);
        _context.Clients.AddRange(client1, client2);
        await _context.SaveChangesAsync();

        _context.Transactions.AddRange(
            new Transaction 
            { 
                Id = 1, 
                PropertyId = 1, 
                ClientId = 1, 
                Amount = 250000, 
                TransactionType = "Sale",
                Date = DateTime.UtcNow
            },
            new Transaction 
            { 
                Id = 2, 
                PropertyId = 2, 
                ClientId = 2, 
                Amount = 350000, 
                TransactionType = "Rent",
                Date = DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllTransactionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ExistingTransaction_ReturnsTransaction()
    {
        // Arrange
        var property = new Property { Id = 1, Address = "123 Main St", Price = 250000, Type = "Sale", OwnerId = 1 };
        var client = new Client { Id = 1, Name = "John Doe", Email = "john@test.com" };
        _context.Properties.Add(property);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var transaction = new Transaction 
        { 
            Id = 1, 
            PropertyId = 1, 
            ClientId = 1, 
            Amount = 250000, 
            TransactionType = "Sale",
            Date = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTransactionByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(250000, result.Amount);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_NonExistingTransaction_ReturnsNull()
    {
        // Act
        var result = await _service.GetTransactionByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTransactionAsync_ValidTransaction_ReturnsCreatedTransaction()
    {
        // Arrange
        var property = new Property { Id = 1, Address = "123 Main St", Price = 250000, Type = "Sale", OwnerId = 1 };
        var client = new Client { Id = 1, Name = "John Doe", Email = "john@test.com" };
        _context.Properties.Add(property);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var createDto = new CreateTransactionDto
        {
            PropertyId = 1,
            ClientId = 1,
            Amount = 250000,
            TransactionType = "Sale"
        };

        // Act
        var result = await _service.CreateTransactionAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(250000, result.Amount);
        Assert.Equal("Sale", result.TransactionType);
        
        // Verify it was saved to database
        var savedTransaction = await _context.Transactions.FindAsync(result.Id);
        Assert.NotNull(savedTransaction);
    }
}
