using Application.Features.TelegramAccounts.Commands.Create;
using Application.Features.TelegramAccounts.Commands.Delete;
using Application.Features.TelegramAccounts.Queries.GetAll;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.DTOs;

namespace Presentation.Controllers;

[Route("api/telegram-accounts")]
[Authorize]
public class TelegramAccountsController : coo.Presentation.Common.Base.BaseApiController
{
    public TelegramAccountsController(ISender sender) : base(sender) { }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllTelegramAccountsQuery(), ct);
        return Ok(new ApiSuccessResult<List<TelegramAccount>> { Data = result });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTelegramAccountCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return Ok(new ApiSuccessResult<string> { Data = id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _sender.Send(new DeleteTelegramAccountCommand(id), ct);
        return NoContent();
    }
}
