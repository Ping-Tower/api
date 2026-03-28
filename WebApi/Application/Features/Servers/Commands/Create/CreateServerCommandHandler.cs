using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Commands.Create;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, string>
{
    private readonly IServerRepository _serverRepository;
    private readonly IUserContext _userContext;

    public CreateServerCommandHandler(IServerRepository serverRepository, IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _userContext = userContext;
    }

    public async Task<string> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        var server = new Server
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Host = request.Host,
            Port = request.Port,
            Protocol = request.Protocol,
            UserId = _userContext.UserId,
            IsActive = true
        };

        await _serverRepository.CreateAsync(server, cancellationToken);

        return server.Id;
    }
}
