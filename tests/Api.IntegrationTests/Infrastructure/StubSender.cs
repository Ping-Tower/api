using MediatR;

namespace Api.IntegrationTests;

public sealed class StubSender : ISender
{
    public Func<object, CancellationToken, Task<object?>> Handler { get; set; } =
        static (_, _) => Task.FromException<object?>(new InvalidOperationException("ISender handler was not configured for this test."));

    public void Reset()
    {
        Handler = static (_, _) => Task.FromException<object?>(new InvalidOperationException("ISender handler was not configured for this test."));
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var result = await Handler(request, cancellationToken);
        return (TResponse)result!;
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        await Handler(request, cancellationToken);
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default) =>
        Handler(request, cancellationToken);

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public IAsyncEnumerable<object?> CreateStream(
        object request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}
