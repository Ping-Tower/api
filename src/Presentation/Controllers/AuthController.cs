using Application.Common.Services.IdentityManager;
using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.Refresh;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResendVerificationCode;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.VerifyEmail;
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
        return Ok(new ApiSuccessResult<RegistrationResultDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<LoginResultDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<RefreshResultDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<LoginResultDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<ForgotPasswordResultDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<ResetPasswordResultDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPost("resend-verification-code")]
    public async Task<IActionResult> ResendVerificationCode([FromBody] ResendVerificationCodeCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<ResendVerificationCodeResultDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return NoContent();
    }
}
