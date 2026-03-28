using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegistrationResultDto>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<RegistrationResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RegistrationAsync(request.Email, request.Password, request.Name, cancellationToken);
    }
}
