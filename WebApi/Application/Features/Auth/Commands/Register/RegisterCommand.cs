using Application.Common.CQRS;
using Application.Common.Services.IdentityManager;
using MediatR;

namespace Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<RegistrationResultDto>, ICommand;
