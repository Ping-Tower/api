using Application.Common.CQRS;
using MediatR;

namespace Application.Features.TelegramAccounts.Commands.Create;

public record CreateTelegramAccountCommand(string ChatId) : IRequest<string>, ICommand;
