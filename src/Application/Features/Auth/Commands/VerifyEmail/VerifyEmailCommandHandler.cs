using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResultDto>
{
    private readonly IIdentityService _identityService;

    public VerifyEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<VerifyEmailResultDto> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) =>
        _identityService.VerifyEmail(request.Email, request.Code, cancellationToken);
}
