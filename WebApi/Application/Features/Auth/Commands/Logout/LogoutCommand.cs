using Application.Common.CQRS;
using MediatR;

namespace Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Unit>, ICommand;
