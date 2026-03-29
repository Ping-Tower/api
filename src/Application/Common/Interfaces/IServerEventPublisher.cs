using Domain.Entities;
using Application.Common.DTOs;

namespace Application.Common.Interfaces;

public interface IServerEventPublisher
{
    Task PublishServerAddedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken);
    Task PublishServerEditedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken);
    Task PublishServerDeletedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken);
}
