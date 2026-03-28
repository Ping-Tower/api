using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.TelegramAccounts.Queries.GetAll;

public class GetAllTelegramAccountsQueryHandler : IRequestHandler<GetAllTelegramAccountsQuery, List<TelegramAccount>>
{
    private readonly ITelegramAccountRepository _telegramAccountRepository;
    private readonly IUserContext _userContext;

    public GetAllTelegramAccountsQueryHandler(ITelegramAccountRepository telegramAccountRepository, IUserContext userContext)
    {
        _telegramAccountRepository = telegramAccountRepository;
        _userContext = userContext;
    }

    public async Task<List<TelegramAccount>> Handle(GetAllTelegramAccountsQuery request, CancellationToken cancellationToken)
    {
        return await _telegramAccountRepository.GetAllByUserIdAsync(_userContext.UserId!, cancellationToken);
    }
}
