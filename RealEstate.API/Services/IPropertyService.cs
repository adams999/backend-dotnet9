using RealEstate.API.DTOs;

namespace RealEstate.API.Services;

public interface IPropertyService
{
    Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
    Task<PropertyDto?> GetPropertyByIdAsync(int id);
    Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto propertyDto);
    Task<bool> UpdatePropertyAsync(int id, UpdatePropertyDto propertyDto);
    Task<bool> DeletePropertyAsync(int id);
}
