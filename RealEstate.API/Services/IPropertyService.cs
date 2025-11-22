using RealEstate.API.DTOs;
using RealEstate.API.Models.Pagination;

namespace RealEstate.API.Services;

public interface IPropertyService
{
    Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
    Task<PagedResult<PropertyDto>> GetAllPropertiesAsync(PaginationParams paginationParams);
    Task<PropertyDto?> GetPropertyByIdAsync(int id);
    Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto propertyDto);
    Task<bool> UpdatePropertyAsync(int id, UpdatePropertyDto propertyDto);
    Task<bool> DeletePropertyAsync(int id);
}
