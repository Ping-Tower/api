using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.Pings.Queries.GetChart;

public record GetChartQuery(string ServerId, DateTime From, DateTime To, int BucketSec) : IRequest<ChartDto>, IQuery;
