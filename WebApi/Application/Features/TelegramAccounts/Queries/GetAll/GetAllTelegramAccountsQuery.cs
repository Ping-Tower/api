using Application.Common.CQRS;
using Domain.Entities;
using MediatR;

namespace Application.Features.TelegramAccounts.Queries.GetAll;

public record GetAllTelegramAccountsQuery() : IRequest<List<TelegramAccount>>, IQuery;
