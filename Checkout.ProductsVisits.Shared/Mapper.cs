using System.Reflection;
using Amazon.DynamoDBv2.Model;

namespace Checkout.ProductsVisits.Shared;

public abstract class Mapper<TAggregate, TKey>
    where TAggregate : class
    where TKey : struct
{
    public abstract string PkAttributeName { get; }
    public abstract TAggregate MapFromDocument(Dictionary<string, AttributeValue> document);
    public abstract Dictionary<string, AttributeValue> MapToDocument(TAggregate aggregate);

    protected static void SetProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // Para propriedades com setter privado, usar backing field
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(obj, value);
        }
    }
}
