using Application.Common.CQRS;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Queries.GetById;

public record GetServerByIdQuery(string Id) : IRequest<Server>, IQuery;
