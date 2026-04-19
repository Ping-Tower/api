using Application.Common.CQRS;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Queries.GetAll;

public record GetAllServersQuery(string? Search = null) : IRequest<List<Server>>, IQuery;
