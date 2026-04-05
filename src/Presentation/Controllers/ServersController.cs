using Application.Common.DTOs;
using Application.Features.Pings.Queries.GetPings;
using Application.Features.Servers.Commands.Create;
using Application.Features.Servers.Commands.Delete;
using Application.Features.Servers.Commands.Update;
using Application.Features.Servers.Queries.GetAll;
using Application.Features.Servers.Queries.GetById;
using Application.Features.Settings.Commands.Update;
using Application.Features.Settings.Queries.Get;
using Application.Features.State.Queries.GetState;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.DTOs;

namespace Presentation.Controllers;

[Route("api/servers")]
[Authorize]
public class ServersController : Common.Base.BaseApiController
{
    public ServersController(ISender sender) : base(sender) { }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllServersQuery(), ct);
        return Ok(new ApiSuccessResult<List<Server>>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetServerByIdQuery(id), ct);
        return Ok(new ApiSuccessResult<Server>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServerCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiSuccessResult<Server>
        {
            Code = StatusCodes.Status201Created,
            Message = "Created",
            Data = result
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateServerCommandBody body, CancellationToken ct)
    {
        var result = await _sender.Send(new UpdateServerCommand(id, body.Name, body.Host, body.Query, body.Port, body.Protocol), ct);
        return Ok(new ApiSuccessResult<Server>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _sender.Send(new DeleteServerCommand(id), ct);
        return NoContent();
    }

    [HttpGet("{id}/settings")]
    public async Task<IActionResult> GetSettings(string id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetSettingsQuery(id), ct);
        return Ok(new ApiSuccessResult<SettingsDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpPatch("{id}/settings")]
    public async Task<IActionResult> UpdateSettings(string id, [FromBody] UpdateSettingsBody body, CancellationToken ct)
    {
        var result = await _sender.Send(new UpdateSettingsCommand(id, body.IntervalSec, body.LatencyThresholdMs, body.Retries, body.FailureThreshold), ct);
        return Ok(new ApiSuccessResult<SettingsDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpGet("{id}/state")]
    public async Task<IActionResult> GetState(string id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetServerStateQuery(id), ct);
        return Ok(new ApiSuccessResult<ServerStateDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpGet("{id}/pings")]
    public async Task<IActionResult> GetPings(
        string id,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetPingsQuery(id, from, to, limit), ct);
        return Ok(new ApiSuccessResult<List<PingRecord>>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }
}

public record UpdateServerCommandBody(string Name, string Host, string? Query, int Port, Protocol Protocol);
public record UpdateSettingsBody(int? IntervalSec, int? LatencyThresholdMs, int? Retries, int? FailureThreshold);
