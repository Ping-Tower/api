using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Queries.GetById;

public class GetServerByIdQueryHandler : IRequestHandler<GetServerByIdQuery, Server>
{
    private readonly IServerRepository _serverRepository;
    private readonly IUserContext _userContext;

    public GetServerByIdQueryHandler(IServerRepository serverRepository, IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _userContext = userContext;
    }

    public async Task<Server> Handle(GetServerByIdQuery request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Server {request.Id} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizationException("Access denied.");

        return server;
    }
}
