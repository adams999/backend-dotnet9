using Microsoft.EntityFrameworkCore;
using RealEstate.API.Data;
using RealEstate.API.DTOs;
using RealEstate.API.Models;
using RealEstate.API.Models.Pagination;

namespace RealEstate.API.Services;

public class ClientService : IClientService
{
    private readonly RealEstateDbContext _context;

    public ClientService(RealEstateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
    {
        return await _context.Clients
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            })
            .ToListAsync();
    }

    public async Task<PagedResult<ClientDto>> GetAllClientsAsync(PaginationParams paginationParams)
    {
        var totalCount = await _context.Clients.CountAsync();
        
        var items = await _context.Clients
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            })
            .ToListAsync();

        return new PagedResult<ClientDto>
        {
            Items = items,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize)
        };
    }


    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return null;

        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber
        };
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientDto clientDto)
    {
        var client = new Client
        {
            Name = clientDto.Name,
            Email = clientDto.Email,
            PhoneNumber = clientDto.PhoneNumber
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber
        };
    }

    public async Task<bool> UpdateClientAsync(int id, UpdateClientDto clientDto)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return false;

        client.Name = clientDto.Name;
        client.Email = clientDto.Email;
        client.PhoneNumber = clientDto.PhoneNumber;

        _context.Entry(client).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientExists(id)) return false;
            throw;
        }
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }

    private bool ClientExists(int id)
    {
        return _context.Clients.Any(e => e.Id == id);
    }
}
