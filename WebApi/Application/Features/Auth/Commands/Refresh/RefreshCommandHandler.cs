using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshResultDto>
{
    private readonly IIdentityService _identityService;

    public RefreshCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<RefreshResultDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.Refresh(request.RefreshToken, cancellationToken);
    }
}
