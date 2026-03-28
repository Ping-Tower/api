using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.State.Queries.GetState;

public record GetServerStateQuery(string ServerId) : IRequest<ServerStateDto>, IQuery;
