namespace RealEstate.API.DTOs;

public class PropertyDto
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
}

public class CreatePropertyDto
{
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = "Sale";
    public string? Description { get; set; }
    public int OwnerId { get; set; }
}

public class UpdatePropertyDto
{
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = "Sale";
    public string? Description { get; set; }
}
