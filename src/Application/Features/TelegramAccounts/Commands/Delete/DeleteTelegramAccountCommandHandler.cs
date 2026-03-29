using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.TelegramAccounts.Commands.Delete;

public class DeleteTelegramAccountCommandHandler : IRequestHandler<DeleteTelegramAccountCommand, Unit>
{
    private readonly ITelegramAccountRepository _telegramAccountRepository;
    private readonly IUserContext _userContext;

    public DeleteTelegramAccountCommandHandler(ITelegramAccountRepository telegramAccountRepository, IUserContext userContext)
    {
        _telegramAccountRepository = telegramAccountRepository;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteTelegramAccountCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _telegramAccountRepository.GetAllByUserIdAsync(_userContext.UserId!, cancellationToken);
        var account = accounts.FirstOrDefault(a => a.Id == request.Id)
            ?? throw new NotFoundException($"Telegram account {request.Id} not found.");

        await _telegramAccountRepository.DeleteAsync(account, cancellationToken);

        return Unit.Value;
    }
}
