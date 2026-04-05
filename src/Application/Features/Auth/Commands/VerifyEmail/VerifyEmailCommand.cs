using Application.Common.CQRS;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Email, string Code) : IRequest<VerifyEmailResultDto>, ICommand;
