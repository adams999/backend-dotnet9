using MediatR;
using RealEstate.API.Services;

namespace RealEstate.API.Features.Clients.Commands;

public record DeleteClientCommand(int Id) : IRequest<bool>;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
{
    private readonly IClientService _service;

    public DeleteClientCommandHandler(IClientService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        return await _service.DeleteClientAsync(request.Id);
    }
}
