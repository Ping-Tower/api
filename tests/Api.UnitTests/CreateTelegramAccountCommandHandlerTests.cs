using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Features.TelegramAccounts.Commands.Create;
using Domain.Common.Exceptions;
using Domain.Entities;
using Xunit;

namespace Api.UnitTests;

public class CreateTelegramAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_UpsertsAccountAndSendsWelcomeMessage()
    {
        var repository = new FakeTelegramAccountRepository();
        var validator = new FakeTelegramLoginValidator
        {
            IsValidResult = true,
            AuthDateUtc = new DateTimeOffset(2026, 4, 5, 12, 0, 0, TimeSpan.Zero)
        };
        var notificationService = new FakeTelegramNotificationService();
        var userContext = new FakeUserContext("user-123");

        var handler = new CreateTelegramAccountCommandHandler(
            repository,
            validator,
            notificationService,
            userContext);

        var command = new CreateTelegramAccountCommand(
            123456789,
            "Ivan",
            "ivanov",
            "https://example.com/avatar.png",
            1712311111,
            "hash");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(repository.UpsertedAccount);
        Assert.Same(repository.UpsertedAccount, result);
        Assert.Equal(repository.UpsertedAccount!.Id, result.Id);
        Assert.Equal(command.TelegramUserId, repository.UpsertedAccount.TelegramUserId);
        Assert.Equal(command.FirstName, repository.UpsertedAccount.FirstName);
        Assert.Equal(command.Username, repository.UpsertedAccount.Username);
        Assert.Equal(command.PhotoUrl, repository.UpsertedAccount.PhotoUrl);
        Assert.Equal("user-123", repository.UpsertedAccount.UserId);
        Assert.False(repository.UpsertedAccount.IsDeleted);
        Assert.Equal(validator.AuthDateUtc, repository.UpsertedAccount.AuthDateUtc);

        var sentMessage = Assert.Single(notificationService.Messages);
        Assert.Equal(command.TelegramUserId, sentMessage.ChatId);
        Assert.Equal("✅ <b>Telegram успешно привязан</b>\nТеперь уведомления PingTower будут приходить сюда.", sentMessage.Text);
        Assert.Null(sentMessage.InlineButtons);
    }

    [Fact]
    public async Task Handle_InvalidTelegramLogin_ThrowsAndDoesNotPersistOrNotify()
    {
        var repository = new FakeTelegramAccountRepository();
        var validator = new FakeTelegramLoginValidator
        {
            IsValidResult = false,
            AuthDateUtc = new DateTimeOffset(2026, 4, 5, 12, 0, 0, TimeSpan.Zero)
        };
        var notificationService = new FakeTelegramNotificationService();
        var userContext = new FakeUserContext("user-123");

        var handler = new CreateTelegramAccountCommandHandler(
            repository,
            validator,
            notificationService,
            userContext);

        var command = new CreateTelegramAccountCommand(
            123456789,
            "Ivan",
            "ivanov",
            null,
            1712311111,
            "hash");

        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Null(repository.UpsertedAccount);
        Assert.Empty(notificationService.Messages);
    }

    private sealed class FakeTelegramAccountRepository : ITelegramAccountRepository
    {
        public TelegramAccount? UpsertedAccount { get; private set; }

        public Task<List<TelegramAccount>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken) =>
            Task.FromResult(new List<TelegramAccount>());

        public Task<TelegramAccount?> GetByUserIdAsync(string userId, CancellationToken cancellationToken) =>
            Task.FromResult<TelegramAccount?>(null);

        public Task<TelegramAccount?> GetByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken) =>
            Task.FromResult<TelegramAccount?>(null);

        public Task UpsertAsync(TelegramAccount account, CancellationToken cancellationToken)
        {
            UpsertedAccount = account;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TelegramAccount account, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class FakeTelegramLoginValidator : ITelegramLoginValidator
    {
        public bool IsValidResult { get; init; }
        public DateTimeOffset AuthDateUtc { get; init; }

        public bool IsValid(long telegramUserId, string firstName, string? username, string? photoUrl, long authDate, string hash) =>
            IsValidResult;

        public DateTimeOffset GetAuthDateUtc(long authDate) => AuthDateUtc;
    }

    private sealed class FakeUserContext(string userId) : IUserContext
    {
        public string? UserName => "tester";
        public string? UserId => userId;
        public List<string>? Roles => ["User"];
    }

    private sealed class FakeTelegramNotificationService : ITelegramNotificationService
    {
        public List<SentTelegramMessage> Messages { get; } = [];

        public Task SendMessageAsync(
            long chatId,
            string text,
            IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? inlineButtons,
            CancellationToken cancellationToken)
        {
            Messages.Add(new SentTelegramMessage(chatId, text, inlineButtons));
            return Task.CompletedTask;
        }
    }

    private sealed record SentTelegramMessage(
        long ChatId,
        string Text,
        IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? InlineButtons);
}
