namespace Domain.Common.Interfaces;

public interface IHasAudience
{
    DateTimeOffset CreatedAt { get; init; }
    string? CreatedBy { get; init; }
    DateTimeOffset ModifiedAt { get; set; }
    string? ModifiedBy { get; set; }
}