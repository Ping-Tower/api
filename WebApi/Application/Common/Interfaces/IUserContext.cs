namespace Application.Common.Interfaces;

public interface IUserContext
{
    string? UserName { get; }
    string? UserId { get; }
    List<string>? Roles { get; }
}