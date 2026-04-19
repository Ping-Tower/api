using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Queries.GetAll;

public class GetAllServersQueryHandler : IRequestHandler<GetAllServersQuery, List<Server>>
{
    private readonly IServerRepository _serverRepository;
    private readonly IUserContext _userContext;

    public GetAllServersQueryHandler(IServerRepository serverRepository, IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _userContext = userContext;
    }

    public async Task<List<Server>> Handle(GetAllServersQuery request, CancellationToken cancellationToken)
    {
        return await _serverRepository.GetAllByUserIdAsync(_userContext.UserId!, request.Search, cancellationToken);
    }
}
