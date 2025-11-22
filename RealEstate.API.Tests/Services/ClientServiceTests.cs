using Microsoft.EntityFrameworkCore;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;
using RealEstate.API.Services;

namespace RealEstate.API.Tests.Services;

public class ClientServiceTests : IDisposable
{
    private readonly RealEstateDbContext _context;
    private readonly ClientService _service;

    public ClientServiceTests()
    {
        var options = new DbContextOptionsBuilder<RealEstateDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RealEstateDbContext(options);
        _service = new ClientService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllClientsAsync_ReturnsAllClients()
    {
        // Arrange
        _context.Clients.AddRange(
            new Client { Id = 1, Name = "Client 1", Email = "client1@test.com", PhoneNumber = "123" },
            new Client { Id = 2, Name = "Client 2", Email = "client2@test.com", PhoneNumber = "456" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllClientsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetClientByIdAsync_ExistingClient_ReturnsClient()
    {
        // Arrange
        var client = new Client { Id = 1, Name = "Test Client", Email = "test@test.com", PhoneNumber = "123" };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetClientByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Client", result.Name);
    }

    [Fact]
    public async Task GetClientByIdAsync_NonExistingClient_ReturnsNull()
    {
        // Act
        var result = await _service.GetClientByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateClientAsync_ValidClient_ReturnsCreatedClient()
    {
        // Arrange
        var createDto = new CreateClientDto
        {
            Name = "New Client",
            Email = "new@test.com",
            PhoneNumber = "789"
        };

        // Act
        var result = await _service.CreateClientAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Client", result.Name);
        Assert.Equal("new@test.com", result.Email);
        
        // Verify it was saved to database
        var savedClient = await _context.Clients.FindAsync(result.Id);
        Assert.NotNull(savedClient);
    }

    [Fact]
    public async Task DeleteClientAsync_ExistingClient_ReturnsTrue()
    {
        // Arrange
        var client = new Client { Id = 1, Name = "Test Client", Email = "test@test.com" };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteClientAsync(1);

        // Assert
        Assert.True(result);
        
        // Verify it was deleted
        var deletedClient = await _context.Clients.FindAsync(1);
        Assert.Null(deletedClient);
    }

    [Fact]
    public async Task DeleteClientAsync_NonExistingClient_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteClientAsync(999);

        // Assert
        Assert.False(result);
    }
}

