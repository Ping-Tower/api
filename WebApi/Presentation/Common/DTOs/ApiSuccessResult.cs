namespace Presentation.Common.DTOs;

public class ApiSuccessResult<T>
{
    public int? Code { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
}
