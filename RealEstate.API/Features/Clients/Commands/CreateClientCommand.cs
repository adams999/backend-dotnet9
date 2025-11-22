using MediatR;
using RealEstate.API.DTOs;
using RealEstate.API.Services;

namespace RealEstate.API.Features.Clients.Commands;

public record CreateClientCommand(CreateClientDto ClientDto) : IRequest<ClientDto>;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientService _service;

    public CreateClientCommandHandler(IClientService service)
    {
        _service = service;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        return await _service.CreateClientAsync(request.ClientDto);
    }
}
