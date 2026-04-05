using Application.Common.CQRS;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Servers.Commands.Update;

public record UpdateServerCommand(string Id, string Name, string Host, string? Query, int Port, Protocol Protocol) : IRequest<Server>, ICommand;
