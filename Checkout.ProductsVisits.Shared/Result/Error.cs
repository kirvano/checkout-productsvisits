namespace Checkout.ProductsVisits.Shared.Result;

public sealed record Error(string Code, string Message)
{
    public static Error Unexpected => new("unexpected", "An unexpected error occurred.");
    public static Error None => new(string.Empty, string.Empty);

    public static Error Failure(string code, string message) => new(code, message);
}