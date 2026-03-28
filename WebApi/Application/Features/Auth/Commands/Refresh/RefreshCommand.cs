using Application.Common.CQRS;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<RefreshResultDto>, ICommand;
