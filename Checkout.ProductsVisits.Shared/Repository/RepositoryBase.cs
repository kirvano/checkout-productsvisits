using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Checkout.ProductsVisits.Shared.Configurations;
using Checkout.ProductsVisits.Shared.Repository.Interface;
using Microsoft.Extensions.Logging;

namespace Checkout.ProductsVisits.Shared.Repository;

public abstract class RepositoryBase<TAggregate, TKey>(
    IAmazonDynamoDB client,
    Mapper<TAggregate, TKey> mapper,
    DynamoDbConfig config,
    ILogger<RepositoryBase<TAggregate, TKey>> logger,
    string tableName = "DefaultTable") : IRepository<TAggregate, TKey>
    where TAggregate : class
    where TKey : struct
{
    protected readonly string TableName = config.GetTableName(tableName);

    public async Task<bool> Exists(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [mapper.PkAttributeName] = new() { S = id.ToString() }
                }
            };

            var response = await client.GetItemAsync(request, cancellationToken);
            return response.Item?.Count > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking existence of {AggregateName} {Id}", typeof(TAggregate).Name, id);
            throw;
        }
    }

    public async Task<TAggregate> Get(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [mapper.PkAttributeName] = new() { S = id.ToString() }
                }
            };

            var response = await client.GetItemAsync(request, cancellationToken);
            return mapper.MapFromDocument(response.Item);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving {AggregateName} {Id}", typeof(TAggregate).Name, id);
            throw;
        }
    }

    public async Task Upsert(TAggregate entity, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = mapper.MapToDocument(entity);

            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = document
            };

            await client.PutItemAsync(request, cancellationToken);

            var id = GetEntityId(entity);
            logger.LogInformation("{AggregateName} {Id} saved successfully", typeof(TAggregate).Name, id);
        }
        catch (Exception ex)
        {
            var id = GetEntityId(entity);
            logger.LogError(ex, "Error saving {AggregateName} {Id}", typeof(TAggregate).Name, id);
            throw;
        }
    }

    private static object? GetEntityId(TAggregate entity)
    {
        var idProperty = typeof(TAggregate).GetProperty("Id");
        return idProperty?.GetValue(entity);
    }
}