using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.API.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    public int PropertyId { get; set; }
    public Property? Property { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public string TransactionType { get; set; } = "Sale"; // Sale or Rent
}
