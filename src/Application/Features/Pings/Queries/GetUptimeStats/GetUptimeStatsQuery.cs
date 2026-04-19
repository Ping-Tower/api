using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.Pings.Queries.GetUptimeStats;

public record GetUptimeStatsQuery(string ServerId, DateTime From, DateTime To) : IRequest<UptimeStatsDto>, IQuery;
