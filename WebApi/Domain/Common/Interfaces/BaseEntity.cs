namespace Domain.Common.Interfaces;

public abstract class BaseEntity : IHasId<string?>, IHasAudience
{
    public string? Id { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTimeOffset ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; } 
}