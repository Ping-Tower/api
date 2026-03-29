using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Servers.Commands.Update;

public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, Unit>
{
    private readonly IServerRepository _serverRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IServerEventPublisher _serverEventPublisher;
    private readonly IUserContext _userContext;

    public UpdateServerCommandHandler(
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

    public async Task<Unit> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Server {request.Id} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        server.Name = request.Name;
        server.Host = request.Host;
        server.Port = request.Port;
        server.Protocol = request.Protocol;

        await _serverRepository.UpdateAsync(server, cancellationToken);
        var pingSettings = await _settingsRepository.GetPingSettingsByServerIdAsync(server.Id!, cancellationToken)
            ?? throw new NotFoundException($"Ping settings for server {server.Id} not found.");
        await _serverEventPublisher.PublishServerEditedAsync(server, pingSettings, cancellationToken);

        return Unit.Value;
    }
}
