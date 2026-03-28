using Application.Common.CQRS;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Queries.GetAll;

public record GetAllServersQuery() : IRequest<List<Server>>, IQuery;
