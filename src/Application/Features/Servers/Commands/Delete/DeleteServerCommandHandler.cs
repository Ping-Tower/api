using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Servers.Commands.Delete;

public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, Unit>
{
    private readonly IServerRepository _serverRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IServerEventPublisher _serverEventPublisher;
    private readonly IUserContext _userContext;

    public DeleteServerCommandHandler(
        IServerRepository serverRepository,
        ISettingsRepository settingsRepository,
        IServerEventPublisher serverEventPublisher,
        IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _settingsRepository = settingsRepository;
        _serverEventPublisher = serverEventPublisher;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Server {request.Id} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        await _serverRepository.DeleteAsync(server, cancellationToken);
        var pingSettings = await _settingsRepository.GetPingSettingsByServerIdAsync(server.Id!, cancellationToken)
            ?? new Domain.Entities.PingSettings
            {
                Id = Guid.NewGuid().ToString(),
                ServerId = server.Id
            };
        await _serverEventPublisher.PublishServerDeletedAsync(server, pingSettings, cancellationToken);

        return Unit.Value;
    }
}
