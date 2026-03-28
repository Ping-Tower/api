using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.TelegramAccounts.Commands.Create;

public class CreateTelegramAccountCommandHandler : IRequestHandler<CreateTelegramAccountCommand, string>
{
    private readonly ITelegramAccountRepository _telegramAccountRepository;
    private readonly IUserContext _userContext;

    public CreateTelegramAccountCommandHandler(ITelegramAccountRepository telegramAccountRepository, IUserContext userContext)
    {
        _telegramAccountRepository = telegramAccountRepository;
        _userContext = userContext;
    }

    public async Task<string> Handle(CreateTelegramAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new TelegramAccount
        {
            Id = Guid.NewGuid().ToString(),
            ChatId = request.ChatId,
            UserId = _userContext.UserId
        };

        await _telegramAccountRepository.CreateAsync(account, cancellationToken);

        return account.Id;
    }
}
