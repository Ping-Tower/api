using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.Pings.Queries.GetPings;

public record GetPingsQuery(
    string ServerId,
    DateTime? From,
    DateTime? To,
    int Limit = 50) : IRequest<List<PingRecord>>, IQuery;
