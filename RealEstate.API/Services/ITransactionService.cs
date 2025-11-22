using RealEstate.API.DTOs;
using RealEstate.API.Models.Pagination;

namespace RealEstate.API.Services;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
    Task<PagedResult<TransactionDto>> GetAllTransactionsAsync(PaginationParams paginationParams);
    Task<TransactionDto?> GetTransactionByIdAsync(int id);
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto);
}
