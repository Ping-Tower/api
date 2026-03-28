using Application.Common.CQRS;
using Application.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResultDto>, ICommand;
