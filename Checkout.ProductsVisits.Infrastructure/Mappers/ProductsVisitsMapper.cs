using Amazon.DynamoDBv2.Model;
using Checkout.ProductsVisits.Domain.Aggregate;
using Checkout.ProductsVisits.Shared;

namespace Checkout.ProductsVisits.Infrastructure.Mappers;

public class ProductsVisitsMapper : Mapper<ProductVisit, long>
{
    public override string PkAttributeName => "product_id";

    public override ProductVisit MapFromDocument(Dictionary<string, AttributeValue> document)
    {
        if (document == null || document.Count == 0)
            throw new ArgumentNullException(nameof(document));

        // Usando reflection para acessar o construtor privado e propriedades
        var productVisit = (ProductVisit)Activator.CreateInstance(typeof(ProductVisit), true)!;

        // Mapeamento dos campos básicos
        if (!long.TryParse(document[PkAttributeName].S, out var id))
            throw new InvalidOperationException($"Invalid product_id format: {document[PkAttributeName].S}");

        SetProperty(productVisit, nameof(ProductVisit.Id), id);

        SetProperty(productVisit, nameof(ProductVisit.Day),
            DateOnly.Parse(document[nameof(ProductVisit.Day)].S));


        if (!long.TryParse(document[nameof(ProductVisit.CountVisits)].S, out var count))
            throw new InvalidOperationException(
                $"Invalid CountVisits format: {document[nameof(ProductVisit.CountVisits)].S}");
        SetProperty(productVisit, nameof(ProductVisit.CountVisits), count);

        return productVisit;
    }

    public override Dictionary<string, AttributeValue> MapToDocument(ProductVisit aggregate)
    {
        ArgumentNullException.ThrowIfNull(aggregate);
        var document = new Dictionary<string, AttributeValue>
        {
            [PkAttributeName] = new() { S = aggregate.Id.ToString() },
            [nameof(ProductVisit.Day)] = new() { S = aggregate.Day.ToString() },
            [nameof(ProductVisit.CountVisits)] = new() { S = aggregate.CountVisits.ToString() },
        };

        return document;
    }
}