using Application.Common.Interfaces;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Queries.GetMe;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, CurrentUserDto>
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;

    public GetMeQueryHandler(IIdentityService identityService, IUserContext userContext)
    {
        _identityService = identityService;
        _userContext = userContext;
    }

    public async Task<CurrentUserDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetCurrentUserAsync(_userContext.UserId!, cancellationToken);
    }
}
