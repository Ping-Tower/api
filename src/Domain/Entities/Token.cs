using Domain.Common.Interfaces;

namespace Domain.Entities;

public class Token : BaseEntity
{
    public string? UserId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expire { get; set; }
}