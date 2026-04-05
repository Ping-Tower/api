using Application.Common.CQRS;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResultDto>, ICommand;
