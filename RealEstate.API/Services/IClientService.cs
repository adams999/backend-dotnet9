using RealEstate.API.DTOs;
using RealEstate.API.Models.Pagination;

namespace RealEstate.API.Services;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllClientsAsync();
    Task<PagedResult<ClientDto>> GetAllClientsAsync(PaginationParams paginationParams);
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto clientDto);
    Task<bool> UpdateClientAsync(int id, UpdateClientDto clientDto);
    Task<bool> DeleteClientAsync(int id);
}
