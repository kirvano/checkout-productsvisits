using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Checkout.ProductsVisits.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.ProductsVisits.Shared.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDynamoDbService(this IServiceCollection services,
        IConfiguration configuration,
        bool includeHealthCheck = false)
    {
        var dynamoSection = configuration.GetSection(DynamoDbConfig.DynamoDb);
        var dynamoDbConfig = dynamoSection.Get<DynamoDbConfig>() ?? new DynamoDbConfig();

        if (string.IsNullOrEmpty(dynamoDbConfig.Region))
        {
            throw new ArgumentException("DynamoDB Region configuration is required");
        }

        var region = RegionEndpoint.GetBySystemName(dynamoDbConfig.Region);

        if (includeHealthCheck)
        {
            services.AddHealthChecks()
                .AddDynamoDb(opt => opt.RegionEndpoint = region);
        }

        services.Configure<DynamoDbConfig>(dynamoSection);

        return dynamoDbConfig.Environment == "Local"
            ? services.ConfigureLocalStack(dynamoDbConfig, region)
            : services.ConfigureAwsEnvironment(region);
    }

    private static IServiceCollection ConfigureAwsEnvironment(
        this IServiceCollection services,
        RegionEndpoint region)
    {
        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = region
        };
        services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(config));
        return services;
    }

    private static IServiceCollection ConfigureLocalStack(
        this IServiceCollection services,
        DynamoDbConfig dynamoDbConfig,
        RegionEndpoint region)
    {
        // Validate AWS credentials configuration
        var hasAccessKey = !string.IsNullOrEmpty(dynamoDbConfig.AccessKey);
        var hasSecretKey = !string.IsNullOrEmpty(dynamoDbConfig.SecretKey);

        switch (hasAccessKey)
        {
            case true when !hasSecretKey:
                throw new ArgumentException("DynamoDB SecretKey is required when AccessKey is provided");
            case false when hasSecretKey:
                throw new ArgumentException("DynamoDB AccessKey is required when SecretKey is provided");
            case false when !hasSecretKey:
                System.Diagnostics.Debug.WriteLine(
                    "WARNING: No AWS credentials provided in configuration. Will attempt to use default credential chain (environment variables, AWS CLI profile, IAM roles, etc.)");
                break;
        }

        // Create DynamoDB client based on configuration
        IAmazonDynamoDB CreateDynamoDbClient()
        {
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = region
            };

            // Set ServiceURL for local DynamoDB development
            // Note: "DISABLED" is used in AWS Parameter Store (which doesn't accept empty strings)
            // to indicate "use AWS DynamoDB, not LocalStack"
            if (!string.IsNullOrEmpty(dynamoDbConfig.ServiceUrl) &&
                !dynamoDbConfig.ServiceUrl.Equals("DISABLED", StringComparison.OrdinalIgnoreCase))
            {
                config.ServiceURL = dynamoDbConfig.ServiceUrl;
                System.Diagnostics.Debug.WriteLine(
                    $"DEBUG: DynamoDB ServiceURL configured: {dynamoDbConfig.ServiceUrl}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    $"DEBUG: DynamoDB using AWS cloud endpoint for region: {dynamoDbConfig.Region}");
            }

            // Use AWS credentials if provided
            if (!string.IsNullOrEmpty(dynamoDbConfig.AccessKey) && !string.IsNullOrEmpty(dynamoDbConfig.SecretKey))
            {
                var credentials = new BasicAWSCredentials(dynamoDbConfig.AccessKey, dynamoDbConfig.SecretKey);
                System.Diagnostics.Debug.WriteLine("DEBUG: Using provided AWS credentials");
                return new AmazonDynamoDBClient(credentials, config);
            }

            // Use default credential chain (environment variables, IAM roles, etc.)
            System.Diagnostics.Debug.WriteLine("DEBUG: Using default AWS credential chain");
            return new AmazonDynamoDBClient(config);
        }

        services.AddSingleton(CreateDynamoDbClient());
        return services;
    }
}