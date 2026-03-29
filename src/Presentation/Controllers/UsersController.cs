using Application.Common.Services.IdentityManager;
using Application.Features.Auth.Queries.GetMe;
using Application.Features.NotificationSettings.Commands.Patch;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.DTOs;

namespace Presentation.Controllers;

[Route("api/users")]
[Authorize]
public class UsersController : Presentation.Common.Base.BaseApiController
{
    public UsersController(ISender sender) : base(sender) { }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var result = await _sender.Send(new GetMeQuery(), ct);
        return Ok(new ApiSuccessResult<CurrentUserDto> { Data = result });
    }

    [HttpPatch("notification-settings")]
    public async Task<IActionResult> PatchNotificationSettings([FromBody] PatchNotificationSettingsCommand command, CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return NoContent();
    }
}
