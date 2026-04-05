using Application.Common.CQRS;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.ResendVerificationCode;

public record ResendVerificationCodeCommand(string Email) : IRequest<ResendVerificationCodeResultDto>, ICommand;
