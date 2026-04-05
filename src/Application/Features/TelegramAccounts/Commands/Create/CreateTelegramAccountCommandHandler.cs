using Application.Common.Interfaces;
using Domain.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Features.TelegramAccounts.Commands.Create;

public class CreateTelegramAccountCommandHandler : IRequestHandler<CreateTelegramAccountCommand, TelegramAccount>
{
    private readonly ITelegramAccountRepository _telegramAccountRepository;
    private readonly ITelegramLoginValidator _telegramLoginValidator;
    private readonly ITelegramNotificationService _telegramNotificationService;
    private readonly IUserContext _userContext;

    public CreateTelegramAccountCommandHandler(
        ITelegramAccountRepository telegramAccountRepository,
        ITelegramLoginValidator telegramLoginValidator,
        ITelegramNotificationService telegramNotificationService,
        IUserContext userContext)
    {
        _telegramAccountRepository = telegramAccountRepository;
        _telegramLoginValidator = telegramLoginValidator;
        _telegramNotificationService = telegramNotificationService;
        _userContext = userContext;
    }

    public async Task<TelegramAccount> Handle(CreateTelegramAccountCommand request, CancellationToken cancellationToken)
    {
        if (!_telegramLoginValidator.IsValid(
                request.TelegramUserId,
                request.FirstName,
                request.Username,
                request.PhotoUrl,
                request.AuthDate,
                request.Hash))
        {
            throw new DomainException("telegram", ["Telegram login data is invalid or expired."]);
        }

        var existingByTelegramId = await _telegramAccountRepository.GetByTelegramUserIdAsync(request.TelegramUserId, cancellationToken);
        if (existingByTelegramId is not null && existingByTelegramId.UserId != _userContext.UserId && !existingByTelegramId.IsDeleted)
            throw new DomainException("telegram", ["Telegram account is already linked to another user."]);

        var existingByUserId = await _telegramAccountRepository.GetByUserIdAsync(_userContext.UserId!, cancellationToken);

        var account = existingByUserId
            ?? existingByTelegramId
            ?? new TelegramAccount
            {
                Id = Guid.NewGuid().ToString()
            };

        account.TelegramUserId = request.TelegramUserId;
        account.FirstName = request.FirstName;
        account.Username = request.Username;
        account.PhotoUrl = request.PhotoUrl;
        account.AuthDateUtc = _telegramLoginValidator.GetAuthDateUtc(request.AuthDate);
        account.UserId = _userContext.UserId;
        account.IsDeleted = false;

        await _telegramAccountRepository.UpsertAsync(account, cancellationToken);
        await _telegramNotificationService.SendMessageAsync(
            request.TelegramUserId,
            "✅ <b>Telegram успешно привязан</b>\nТеперь уведомления PingTower будут приходить сюда.",
            null,
            cancellationToken);

        return account;
    }
}
