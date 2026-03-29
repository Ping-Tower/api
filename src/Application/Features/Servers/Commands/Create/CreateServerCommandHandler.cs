using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Servers.Commands.Create;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, string>
{
    private readonly IServerRepository _serverRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IServerEventPublisher _serverEventPublisher;
    private readonly IUserContext _userContext;

    public CreateServerCommandHandler(
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

    public async Task<string> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        var server = new Server
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Host = request.Host,
            Port = request.Port,
            Protocol = request.Protocol,
            UserId = _userContext.UserId,
            IsActive = true,
            Status = ServerStatus.UNKNOWN
        };

        await _serverRepository.CreateAsync(server, cancellationToken);

        var pingSettings = new PingSettings
        {
            Id = Guid.NewGuid().ToString(),
            ServerId = server.Id,
            IntervalSec = 60,
            LatencyThresholdMs = 400,
            Retries = 0,
            FailureThreshold = 1
        };

        await _settingsRepository.UpsertPingSettingsAsync(pingSettings, cancellationToken);
        await _serverEventPublisher.PublishServerAddedAsync(server, pingSettings, cancellationToken);

        return server.Id;
    }
}
