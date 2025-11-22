using RealEstate.API.DTOs;

namespace RealEstate.API.Services;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllClientsAsync();
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto clientDto);
    Task<bool> UpdateClientAsync(int id, UpdateClientDto clientDto);
    Task<bool> DeleteClientAsync(int id);
}
