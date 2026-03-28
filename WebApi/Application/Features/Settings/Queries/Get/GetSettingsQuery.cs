using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.Settings.Queries.Get;

public record GetSettingsQuery(string ServerId) : IRequest<SettingsDto>, IQuery;
