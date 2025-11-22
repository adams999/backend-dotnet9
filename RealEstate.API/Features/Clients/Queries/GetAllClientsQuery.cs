using MediatR;
using RealEstate.API.DTOs;
using RealEstate.API.Services;

namespace RealEstate.API.Features.Clients.Queries;

public record GetAllClientsQuery : IRequest<IEnumerable<ClientDto>>;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientDto>>
{
    private readonly IClientService _service;

    public GetAllClientsQueryHandler(IClientService service)
    {
        _service = service;
    }

    public async Task<IEnumerable<ClientDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetAllClientsAsync();
    }
}
