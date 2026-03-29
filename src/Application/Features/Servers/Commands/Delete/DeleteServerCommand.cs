using Application.Common.CQRS;
using MediatR;

namespace Application.Features.Servers.Commands.Delete;

public record DeleteServerCommand(string Id) : IRequest<Unit>, ICommand;
