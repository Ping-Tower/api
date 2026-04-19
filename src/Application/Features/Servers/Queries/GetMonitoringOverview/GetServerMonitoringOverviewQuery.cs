using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.Servers.Queries.GetMonitoringOverview;

public record GetServerMonitoringOverviewQuery(
    string ServerId,
    DateTime From,
    DateTime To,
    int BucketSec) : IRequest<ServerMonitoringOverviewDto>, IQuery;
