namespace MadWorldNL.Umiko.Functional;

public abstract record Option<T>
{
    public abstract TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none);

    public static Option<T> Some(T value) => new Some<T>(value);
    public static Option<T> None() => new None<T>();
    
    public static implicit operator Option<T>(T value) => new Some<T>(value);
}

public sealed record Some<T>(T Value) : Option<T>
{
    public override TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) => some(Value);
}

public sealed record None<T> : Option<T>
{
    public override TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) => none();
}