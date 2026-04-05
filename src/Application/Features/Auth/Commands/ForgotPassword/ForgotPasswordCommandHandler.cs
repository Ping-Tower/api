using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResultDto>
{
    private readonly IIdentityService _identityService;

    public ForgotPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<ForgotPasswordResultDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken) =>
        _identityService.ForgotPassword(request.Email, cancellationToken);
}
