using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.API.Models;

public class Property
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public string Type { get; set; } = "Sale"; // Sale or Rent

    public string? Description { get; set; }

    public int OwnerId { get; set; }
    
    // Navigation property if we had an Owner model, simplified for now as Client
    public Client? Owner { get; set; }
}
