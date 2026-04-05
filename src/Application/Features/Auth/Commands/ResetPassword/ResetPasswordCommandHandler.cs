using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResultDto>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<ResetPasswordResultDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken) =>
        _identityService.ResetPasswordAsync(request.Email, request.Code, request.NewPassword, cancellationToken);
}
