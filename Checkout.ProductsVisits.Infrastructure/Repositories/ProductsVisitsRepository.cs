using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Checkout.ProductsVisits.Domain.Aggregate;
using Checkout.ProductsVisits.Domain.Repositories;
using Checkout.ProductsVisits.Infrastructure.Mappers;
using Checkout.ProductsVisits.Shared.Configurations;
using Checkout.ProductsVisits.Shared.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Checkout.ProductsVisits.Infrastructure.Repositories;

public class ProductsVisitsRepository(
    IAmazonDynamoDB client,
    ProductsVisitsMapper mapper,
    IOptions<DynamoDbConfig> options,
    ILogger<ProductsVisitsRepository> logger)
    : RepositoryBase<ProductVisit, long>(client, mapper, options.Value, logger, tableName: "Checkout.ProductVisit"),
        IProductsVisitsRepository, IDisposable
{
    private readonly IAmazonDynamoDB _client = client;
    private bool _disposed;

    public async Task<bool> UpdateProductCount(ProductVisit productVisit)
    {
        try
        {
            var request = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "product_id", new AttributeValue { S = productVisit.Id.ToString() } },
                    { "Day", new AttributeValue { S = productVisit.Day.ToString() } },
                },
                UpdateExpression = "SET CountVisits = :countVisits",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":countVisits"] = new AttributeValue { N = productVisit.CountVisits.ToString() }
                }
            };

            await _client.UpdateItemAsync(request);

            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Error while updating product visit {@productId} | Message: {@Message}", productVisit.Id,
                e.Message);
            return false;
        }
    }

    public async Task<ProductVisit?> GetByProductIdAndDay(long productId, string day)
    {
        try
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "product_id", new AttributeValue { S = productId.ToString() } },
                    { "Day", new AttributeValue { S = day } }
                }
            };
            
            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            if (response.Item == null || response.Item.Count == 0)
                return null;
            
            return mapper.MapFromDocument(response.Item);
        }
        catch (Exception e)
        {
            logger.LogError("Error while get product visit {@productid} | Message: {@Message}", productId,
                e.Message);
            throw;
        }
    }


    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}