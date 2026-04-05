using System.Net;
using System.Net.Http.Json;
using Application.Common.Services.IdentityManager;
using Application.Features.Auth.Commands.Register;
using Domain.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.DTOs;
using Xunit;

namespace Api.IntegrationTests;

[Collection(ApiIntegrationCollection.Name)]
public class AuthControllerTests
{
    private readonly PingTowerApiFactory _factory;

    public AuthControllerTests(PingTowerApiFactory factory)
    {
        _factory = factory;
        _factory.Sender.Reset();
    }

    [Fact]
    public async Task Register_ReturnsWrappedSenderPayload()
    {
        _factory.Sender.Handler = (request, _) =>
        {
            Assert.IsType<RegisterCommand>(request);

            object response = new RegistrationResultDto
            {
                UserId = "user-1",
                UserName = "alice"
            };

            return Task.FromResult<object?>(response);
        };

        using var client = _factory.CreateHttpsClient();
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "alice@example.com",
            password = "Secret123!",
            name = "Alice"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiSuccessResult<RegistrationResultDto>>();
        Assert.NotNull(payload);
        Assert.NotNull(payload.Data);
        Assert.Equal("user-1", payload.Data!.UserId);
        Assert.Equal("alice", payload.Data.UserName);
    }

    [Fact]
    public async Task Register_WhenSenderThrowsDomainException_ReturnsProblemDetails()
    {
        _factory.Sender.Handler = (_, _) =>
            Task.FromException<object?>(new DomainException("auth", ["registration failed"]));

        using var client = _factory.CreateHttpsClient();
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "alice@example.com",
            password = "Secret123!",
            name = "Alice"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("Domain Exception", problem.Title);
        Assert.Equal((int)HttpStatusCode.BadRequest, problem.Status);
        Assert.Contains("registration failed", problem.Detail);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateHttpsClient();
        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
