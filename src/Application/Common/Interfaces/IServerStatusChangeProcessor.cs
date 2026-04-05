using Domain.Enums;

namespace Application.Common.Interfaces;

public interface IServerStatusChangeProcessor
{
    Task ProcessAsync(string serverId, ServerStatus status, CancellationToken cancellationToken);
}
