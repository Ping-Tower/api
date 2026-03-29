using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Pings.Queries.GetPings;

public class GetPingsQueryHandler : IRequestHandler<GetPingsQuery, List<PingRecord>>
{
    private readonly IServerRepository _serverRepository;
    private readonly IClickHouseService _clickHouseService;
    private readonly IUserContext _userContext;

    public GetPingsQueryHandler(
        IServerRepository serverRepository,
        IClickHouseService clickHouseService,
        IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _clickHouseService = clickHouseService;
        _userContext = userContext;
    }

    public async Task<List<PingRecord>> Handle(GetPingsQuery request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        return await _clickHouseService.GetPingsAsync(request.ServerId, request.From, request.To, request.Limit, cancellationToken);
    }
}
