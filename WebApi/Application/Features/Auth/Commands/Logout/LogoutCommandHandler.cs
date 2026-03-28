using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public LogoutCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _identityService.LogoutAsync(request.RefreshToken, cancellationToken);
        return Unit.Value;
    }
}
