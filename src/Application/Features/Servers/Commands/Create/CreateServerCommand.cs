using Application.Common.CQRS;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Servers.Commands.Create;

public record CreateServerCommand(string Name, string Host, string? Query, int Port, Protocol Protocol) : IRequest<Server>, ICommand;
