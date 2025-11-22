using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using RealEstate.API.DTOs;
using RealEstate.API.Models.Pagination;
using RealEstate.API.Services;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _service;

    public ClientsController(IClientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ClientDto>>> GetClients([FromQuery] PaginationParams paginationParams)
    {
        var result = await _service.GetAllClientsAsync(paginationParams);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await _service.GetClientByIdAsync(id);

        if (client == null)
        {
            return NotFound();
        }

        return client;
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto clientDto)
    {
        var createdClient = await _service.CreateClientAsync(clientDto);
        return CreatedAtAction(nameof(GetClient), new { id = createdClient.Id }, createdClient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, UpdateClientDto clientDto)
    {
        var result = await _service.UpdateClientAsync(id, clientDto);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var result = await _service.DeleteClientAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
