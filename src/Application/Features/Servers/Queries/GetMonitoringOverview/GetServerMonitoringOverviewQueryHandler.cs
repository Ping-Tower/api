using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Servers.Queries.GetMonitoringOverview;

public class GetServerMonitoringOverviewQueryHandler(
    IServerRepository serverRepository,
    IClickHouseService clickHouseService,
    IUserContext userContext) : IRequestHandler<GetServerMonitoringOverviewQuery, ServerMonitoringOverviewDto>
{
    public async Task<ServerMonitoringOverviewDto> Handle(GetServerMonitoringOverviewQuery request, CancellationToken cancellationToken)
    {
        var server = await serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        var bucketSec = Math.Clamp(request.BucketSec, 1, 86400);

        var uptimeTask = clickHouseService.GetUptimeStatsAsync(request.ServerId, request.From, request.To, cancellationToken);
        var chartTask = clickHouseService.GetChartAsync(
            request.ServerId,
            request.From,
            request.To,
            bucketSec,
            cancellationToken);

        await Task.WhenAll(uptimeTask, chartTask);

        return new ServerMonitoringOverviewDto
        {
            Target = TargetInfoDto.FromEntity(server),
            Summary = await uptimeTask,
            Chart = await chartTask,
        };
    }
}
