namespace MadWorldNL.Umiko.Functional;

public abstract record Result<T>
{
    public abstract TResult Match<TResult>(Func<T, TResult> success, Func<Exception, TResult> failure);

    public static Result<T> Success(T value) => new Success<T>(value);
    public static Result<T> Failure(Exception exception) => new Failure<T>(exception);
}

public sealed record Success<T>(T Value) : Result<T>
{
    public override TResult Match<TResult>(Func<T, TResult> success, Func<Exception, TResult> failure) => success(Value);
}

public sealed record Failure<T>(Exception Exception) : Result<T>
{
    public override TResult Match<TResult>(Func<T, TResult> success, Func<Exception, TResult> failure) => failure(Exception);
}