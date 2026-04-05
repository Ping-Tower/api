using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.ResendVerificationCode;

public class ResendVerificationCodeCommandHandler : IRequestHandler<ResendVerificationCodeCommand, ResendVerificationCodeResultDto>
{
    private readonly IIdentityService _identityService;

    public ResendVerificationCodeCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<ResendVerificationCodeResultDto> Handle(ResendVerificationCodeCommand request, CancellationToken cancellationToken) =>
        _identityService.ResetVerifyEmailCode(request.Email, cancellationToken);
}
