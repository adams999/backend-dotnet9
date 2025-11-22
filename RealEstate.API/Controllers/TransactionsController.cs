using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using RealEstate.API.DTOs;
using RealEstate.API.Models.Pagination;
using RealEstate.API.Services;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetTransactions([FromQuery] PaginationParams paginationParams)
    {
        var result = await _service.GetAllTransactionsAsync(paginationParams);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
    {
        var transaction = await _service.GetTransactionByIdAsync(id);

        if (transaction == null)
        {
            return NotFound();
        }

        return transaction;
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto transactionDto)
    {
        var createdTransaction = await _service.CreateTransactionAsync(transactionDto);
        return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, createdTransaction);
    }
}
