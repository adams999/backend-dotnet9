using Microsoft.EntityFrameworkCore;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;

namespace RealEstate.API.Services;

public class TransactionService : ITransactionService
{
    private readonly RealEstateDbContext _context;

    public TransactionService(RealEstateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Property)
            .Include(t => t.Client)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                PropertyId = t.PropertyId,
                PropertyAddress = t.Property != null ? t.Property.Address : null,
                ClientId = t.ClientId,
                ClientName = t.Client != null ? t.Client.Name : null,
                Date = t.Date,
                Amount = t.Amount,
                TransactionType = t.TransactionType
            })
            .ToListAsync();
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Property)
            .Include(t => t.Client)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null) return null;

        return new TransactionDto
        {
            Id = transaction.Id,
            PropertyId = transaction.PropertyId,
            PropertyAddress = transaction.Property?.Address,
            ClientId = transaction.ClientId,
            ClientName = transaction.Client?.Name,
            Date = transaction.Date,
            Amount = transaction.Amount,
            TransactionType = transaction.TransactionType
        };
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto)
    {
        var transaction = new Transaction
        {
            PropertyId = transactionDto.PropertyId,
            ClientId = transactionDto.ClientId,
            Amount = transactionDto.Amount,
            TransactionType = transactionDto.TransactionType,
            Date = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Reload to get relationships if needed, but for now just return basic info
        return new TransactionDto
        {
            Id = transaction.Id,
            PropertyId = transaction.PropertyId,
            ClientId = transaction.ClientId,
            Date = transaction.Date,
            Amount = transaction.Amount,
            TransactionType = transaction.TransactionType
        };
    }
}
