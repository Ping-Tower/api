using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.State.Queries.GetState;

public class GetServerStateQueryHandler : IRequestHandler<GetServerStateQuery, ServerStateDto>
{
    private readonly IServerRepository _serverRepository;
    private readonly IUserContext _userContext;

    public GetServerStateQueryHandler(
        IServerRepository serverRepository,
        IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _userContext = userContext;
    }

    public async Task<ServerStateDto> Handle(GetServerStateQuery request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        return new ServerStateDto
        {
            Status = server.Status
        };
    }
}
