using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.NotificationSettings.Queries.Get;

public record GetNotificationSettingsQuery() : IRequest<NotificationSettingsDto>, IQuery;
