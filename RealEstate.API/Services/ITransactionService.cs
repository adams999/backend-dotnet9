using RealEstate.API.DTOs;

namespace RealEstate.API.Services;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
    Task<TransactionDto?> GetTransactionByIdAsync(int id);
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto);
}
