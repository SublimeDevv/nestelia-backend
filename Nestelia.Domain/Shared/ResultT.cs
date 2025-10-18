namespace Nestelia.Domain.Shared
{
    public class Result<T>(bool isSuccess, Error error, T? data, string? message) : Result(isSuccess, error, message)
    {
        public T? Data { get; } = data;
    }
}
