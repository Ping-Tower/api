using Application.Common.CQRS;
using MediatR;

namespace Application.Features.TelegramAccounts.Commands.Delete;

public record DeleteTelegramAccountCommand(string Id) : IRequest<Unit>, ICommand;
