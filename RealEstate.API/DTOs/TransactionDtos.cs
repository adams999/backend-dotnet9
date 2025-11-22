namespace RealEstate.API.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string? PropertyAddress { get; set; }
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
}

public class CreateTransactionDto
{
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = "Sale";
}
