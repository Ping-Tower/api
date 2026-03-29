using Application.Common.CQRS;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Queries.GetMe;

public record GetMeQuery() : IRequest<CurrentUserDto>, IQuery;
