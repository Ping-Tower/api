using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.State.Queries.GetState;

public class GetServerStateQueryHandler : IRequestHandler<GetServerStateQuery, ServerStateDto>
{
    private readonly IServerRepository _serverRepository;
    private readonly IServerStateRepository _serverStateRepository;
    private readonly IUserContext _userContext;

    public GetServerStateQueryHandler(
        IServerRepository serverRepository,
        IServerStateRepository serverStateRepository,
        IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _serverStateRepository = serverStateRepository;
        _userContext = userContext;
    }

    public async Task<ServerStateDto> Handle(GetServerStateQuery request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizationException("Access denied.");

        var state = await _serverStateRepository.GetByServerIdAsync(request.ServerId, cancellationToken);

        return new ServerStateDto
        {
            IsUp = state?.IsUp,
            FailCount = state?.FailCount
        };
    }
}
