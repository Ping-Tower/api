namespace Presentation.Common.DTOs;

public class ApiSuccessResult<T>
{
    public required int Code { get; init; }
    public required string Message { get; init; }
    public T? Data { get; init; }
}
