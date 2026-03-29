namespace Domain.Common.Interfaces;

public interface IHasId<T>
{
    T Id { get; set; }
}