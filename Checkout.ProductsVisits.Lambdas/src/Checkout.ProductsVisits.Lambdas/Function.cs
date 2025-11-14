using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Checkout.ProductsVisits.Lambdas.Functions;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Checkout.ProductsVisits.Lambdas;

public class Function
{
    private readonly IServiceProvider _services;

    public Function()
    {
        // Recupera o DI construído no Program.cs
        _services = Program.BuildServices();
    }

    public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        using var scope = _services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<FunctionHandler>();
        await handler.HandleAsync(dynamoEvent, context);
    }
}