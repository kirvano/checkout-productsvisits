using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cortex.Mediator;
using FluentValidation;

// Shared
using Checkout.ProductsVisits.Shared.Extensions;

// Application + Infrastructure
using Checkout.ProductsVisits.Application;
using Checkout.ProductsVisits.Application.UseCases.CreateOrUpdateVisit;
using Checkout.ProductsVisits.Domain.Repositories;
using Checkout.ProductsVisits.Infrastructure;
using Checkout.ProductsVisits.Infrastructure.Mappers;
using Checkout.ProductsVisits.Infrastructure.Repositories;
using Checkout.ProductsVisits.Lambdas.Functions;
using Cortex.Mediator.DependencyInjection;

namespace Checkout.ProductsVisits.Lambdas;

public static class Program
{
    public static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        // Configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Logging
        services.AddLogging();

        // Cortex Mediator
        services.AddCortexMediator(configuration, new[]
        {
            typeof(VisitCommandHandler)
        });

        // Validators
        services.AddValidatorsFromAssembly(typeof(VisitValidation).Assembly);

        // DynamoDB
        services.AddDynamoDbService(configuration);

        // Application + Infrastructure DI
        services.AddScoped<IProductsVisitsRepository,ProductsVisitsRepository>();
        services.AddSingleton<ProductsVisitsMapper>();
        

        // Handler da Lambda
        services.AddScoped<FunctionHandler>();

        return services.BuildServiceProvider();
    }
}