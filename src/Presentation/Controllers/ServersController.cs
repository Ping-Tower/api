using Application.Common.DTOs;
using Application.Features.Pings.Queries.GetPings;
using Application.Features.Pings.Queries.GetUptimeStats;
using Application.Features.Servers.Queries.GetMonitoringOverview;
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
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllServersQuery(search), ct);
        return Ok(new ApiSuccessResult<List<ServerResponseDto>>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result.Select(ServerResponseDto.FromEntity).ToList()
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetServerByIdQuery(id), ct);
        return Ok(new ApiSuccessResult<ServerResponseDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = ServerResponseDto.FromEntity(result)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServerCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiSuccessResult<ServerResponseDto>
        {
            Code = StatusCodes.Status201Created,
            Message = "Created",
            Data = ServerResponseDto.FromEntity(result)
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateServerCommandBody body, CancellationToken ct)
    {
        var result = await _sender.Send(new UpdateServerCommand(id, body.Name, body.Host, body.Query, body.Port, body.Protocol), ct);
        return Ok(new ApiSuccessResult<ServerResponseDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = ServerResponseDto.FromEntity(result)
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

    [HttpGet("{id}/uptime")]
    public async Task<IActionResult> GetUptimeStats(
        string id,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct = default)
    {
        var resolvedTo = to ?? DateTime.UtcNow;
        var resolvedFrom = from ?? resolvedTo.AddHours(-24);
        var result = await _sender.Send(new GetUptimeStatsQuery(id, resolvedFrom, resolvedTo), ct);
        return Ok(new ApiSuccessResult<UptimeStatsDto>
        {
            Code = StatusCodes.Status200OK,
            Message = "OK",
            Data = result
        });
    }

    [HttpGet("{id}/overview")]
    public async Task<IActionResult> GetMonitoringOverview(
        string id,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int? bucketSec,
        CancellationToken ct = default)
    {
        var resolvedTo = to ?? DateTime.UtcNow;
        var resolvedFrom = from ?? resolvedTo.AddHours(-24);
        var resolvedBucketSec = bucketSec ?? ResolveBucketSizeSec(resolvedFrom, resolvedTo);

        var result = await _sender.Send(
            new GetServerMonitoringOverviewQuery(id, resolvedFrom, resolvedTo, resolvedBucketSec),
            ct);

        return Ok(new ApiSuccessResult<ServerMonitoringOverviewDto>
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

    private static int ResolveBucketSizeSec(DateTime from, DateTime to)
    {
        var range = to - from;
        if (range <= TimeSpan.Zero)
            return 1;

        return Math.Clamp((int)Math.Ceiling(range.TotalSeconds / 120d), 1, 86400);
    }
}

public record UpdateServerCommandBody(string Name, string Host, string? Query, int Port, Protocol Protocol);
public record UpdateSettingsBody(int? IntervalSec, int? LatencyThresholdMs, int? Retries, int? FailureThreshold);
