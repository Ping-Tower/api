using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Features.TelegramAccounts.Commands.Create;
using Infrastructure.TelegramManager;
using Xunit;
namespace Api.UnitTests;
public class TelegramLoginValidatorTests
{
    private const string Token = "123456:test-only-token";
    [Theory]
    [InlineData(null)]
    [InlineData("Иванов")]
    public void ValidatesAllTelegramSignedFields(string? lastName)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var payload = new SortedDictionary<string,string>(StringComparer.Ordinal) {
            ["id"] = "123", ["first_name"] = "Иван", ["username"] = "ivan",
            ["auth_date"] = timestamp.ToString(), ["photo_url"] = "https://example.com/photo.jpg"
        };
        if (lastName is not null) payload["last_name"] = lastName;
        var hash = Sign(payload);
        var validator = new TelegramLoginValidator(new TelegramAuthSettings { BotToken = Token });
        Assert.True(validator.IsValid(123,"Иван","ivan","https://example.com/photo.jpg",timestamp,hash,lastName));
        Assert.False(validator.IsValid(123,"Иван","ivan","https://example.com/photo.jpg",timestamp,hash,"Altered"));
        if (lastName is not null)
            Assert.False(validator.IsValid(123,"Иван","ivan","https://example.com/photo.jpg",timestamp,hash));
    }
    [Fact]
    public void DeserializesLastNameFromTelegramJson()
    {
        var command = JsonSerializer.Deserialize<CreateTelegramAccountCommand>("""{"id":123,"first_name":"Ivan","last_name":"Ivanov","auth_date":1,"hash":"hash"}""");
        Assert.Equal("Ivanov", command!.LastName);
    }
    [Fact]
    public void RejectsExpiredOrWrongTokenSignatures()
    {
        var timestamp = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds();
        var payload = new SortedDictionary<string,string> { ["id"]="123",["first_name"]="Ivan",["auth_date"]=timestamp.ToString() };
        var validator = new TelegramLoginValidator(new TelegramAuthSettings { BotToken = Token });
        Assert.False(validator.IsValid(123,"Ivan",null,null,timestamp,Sign(payload)));
        Assert.False(validator.IsValid(123,"Ivan",null,null,DateTimeOffset.UtcNow.ToUnixTimeSeconds(),Sign(payload)));
    }
    private static string Sign(SortedDictionary<string,string> payload) => Convert.ToHexString(HMACSHA256.HashData(SHA256.HashData(Encoding.UTF8.GetBytes(Token)),Encoding.UTF8.GetBytes(string.Join('\n',payload.Select(p=>$"{p.Key}={p.Value}"))))).ToLowerInvariant();
}
