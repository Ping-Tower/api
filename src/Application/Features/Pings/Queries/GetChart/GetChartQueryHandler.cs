using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Pings.Queries.GetChart;

public class GetChartQueryHandler(
    IServerRepository serverRepository,
    IClickHouseService clickHouseService,
    IUserContext userContext) : IRequestHandler<GetChartQuery, ChartDto>
{
    public async Task<ChartDto> Handle(GetChartQuery request, CancellationToken cancellationToken)
    {
        var server = await serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        var bucketSec = Math.Clamp(request.BucketSec, 1, 86400);

        return await clickHouseService.GetChartAsync(
            request.ServerId, request.From, request.To, bucketSec, cancellationToken);
    }
}