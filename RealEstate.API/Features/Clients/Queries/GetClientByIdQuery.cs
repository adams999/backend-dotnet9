using MediatR;
using RealEstate.API.DTOs;
using RealEstate.API.Services;

namespace RealEstate.API.Features.Clients.Queries;

public record GetClientByIdQuery(int Id) : IRequest<ClientDto?>;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientService _service;

    public GetClientByIdQueryHandler(IClientService service)
    {
        _service = service;
    }

    public async Task<ClientDto?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetClientByIdAsync(request.Id);
    }
}
