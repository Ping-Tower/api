using Application.Common.Services.IdentityManager;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.Refresh;
using Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.DTOs;

namespace Presentation.Controllers;

[Route("api/auth")]
public class AuthController : Presentation.Common.Base.BaseApiController
{
    public AuthController(ISender sender) : base(sender) { }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<RegistrationResultDto> { Data = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<LoginResultDto> { Data = result });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<RefreshResultDto> { Data = result });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return NoContent();
    }
}
