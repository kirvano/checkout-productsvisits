namespace Checkout.ProductsVisits.Shared.Configurations;

public class DynamoDbConfig
{
    public const string DynamoDb = "DynamoDB";

    public string Environment { get; init; } = "Local";
    public string Region { get; init; } = "us-east-1";
    public Dictionary<string, string> Tables { get; init; } = [];
    public Dictionary<string, string> Indexes { get; init; } = [];

    public string GetTableName(string tableName)
    {
        return Environment == "Local"
            ? $"{tableName}"
            : Tables[tableName];
    }

    #region LocalStack

    public string ServiceUrl { get; init; } = null;
    public string AccessKey { get; init; } = "test";
    public string SecretKey { get; init; } = "test";
    public string ServiceName { get; init; } = null!;

    #endregion
}