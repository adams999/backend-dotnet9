using Microsoft.EntityFrameworkCore;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;

namespace RealEstate.API.Services;

public class PropertyService : IPropertyService
{
    private readonly RealEstateDbContext _context;

    public PropertyService(RealEstateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
    {
        return await _context.Properties
            .Select(p => new PropertyDto
            {
                Id = p.Id,
                Address = p.Address,
                Price = p.Price,
                Type = p.Type,
                Description = p.Description,
                OwnerId = p.OwnerId
            })
            .ToListAsync();
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null) return null;

        return new PropertyDto
        {
            Id = property.Id,
            Address = property.Address,
            Price = property.Price,
            Type = property.Type,
            Description = property.Description,
            OwnerId = property.OwnerId
        };
    }

    public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto propertyDto)
    {
        var property = new Property
        {
            Address = propertyDto.Address,
            Price = propertyDto.Price,
            Type = propertyDto.Type,
            Description = propertyDto.Description,
            OwnerId = propertyDto.OwnerId
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        return new PropertyDto
        {
            Id = property.Id,
            Address = property.Address,
            Price = property.Price,
            Type = property.Type,
            Description = property.Description,
            OwnerId = property.OwnerId
        };
    }

    public async Task<bool> UpdatePropertyAsync(int id, UpdatePropertyDto propertyDto)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null) return false;

        property.Address = propertyDto.Address;
        property.Price = propertyDto.Price;
        property.Type = propertyDto.Type;
        property.Description = propertyDto.Description;

        _context.Entry(property).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PropertyExists(id)) return false;
            throw;
        }
    }

    public async Task<bool> DeletePropertyAsync(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null) return false;

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();
        return true;
    }

    private bool PropertyExists(int id)
    {
        return _context.Properties.Any(e => e.Id == id);
    }
}
