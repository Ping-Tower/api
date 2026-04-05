using Application.Common.CQRS;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest<ResetPasswordResultDto>, ICommand;
