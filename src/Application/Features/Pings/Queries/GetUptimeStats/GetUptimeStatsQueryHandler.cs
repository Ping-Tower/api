using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Pings.Queries.GetUptimeStats;

public class GetUptimeStatsQueryHandler(
    IServerRepository serverRepository,
    IClickHouseService clickHouseService,
    IUserContext userContext) : IRequestHandler<GetUptimeStatsQuery, UptimeStatsDto>
{
    public async Task<UptimeStatsDto> Handle(GetUptimeStatsQuery request, CancellationToken cancellationToken)
    {
        var server = await serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        return await clickHouseService.GetUptimeStatsAsync(request.ServerId, request.From, request.To, cancellationToken);
    }
}
