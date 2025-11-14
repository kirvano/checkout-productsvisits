namespace Checkout.ProductsVisits.Shared.Result;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("A successful result cannot have an error.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("A failure result must have an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }
    
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static implicit operator Result(Error error) => Failure(error);
}

public sealed class Result<TValue> : Result
{
    private Result(bool isSuccess, Error error) : base(isSuccess, error)
    {
    }

    public TValue? Value { get; private init; }

    public static Result<TValue> Success(TValue value) => new(true, Error.None) { Value = value };

    public new static Result<TValue> Failure(Error error) => new(false, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);

    public static implicit operator Result<TValue>(Error error) => Failure(error);

    public TResult Match<TResult>(Func<TValue?, TResult> value, Func<Error, TResult> error)
        => IsSuccess ? value(Value) : error(Error);
}