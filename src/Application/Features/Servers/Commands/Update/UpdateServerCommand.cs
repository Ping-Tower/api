using Application.Common.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.Features.Servers.Commands.Update;

public record UpdateServerCommand(string Id, string Name, string Host, int Port, Protocol Protocol) : IRequest<Unit>, ICommand;
