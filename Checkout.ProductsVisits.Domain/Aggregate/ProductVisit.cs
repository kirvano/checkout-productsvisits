using Checkout.ProductsVisits.Domain.Aggregate.Base;

namespace Checkout.ProductsVisits.Domain.Aggregate;

public class ProductVisit: AggregateRoot<long>
{
    public DateOnly Day { get; private set; }
    public long CountVisits { get; private set; }
    
    
    private ProductVisit() { }

    public static ProductVisit Create(long productId)
    {
        
        var product = new ProductVisit
        {
            Id = productId,
            Day = DateOnly.FromDateTime(DateTime.UtcNow),
            CountVisits = 1
        };
        return product;
    }

    public void UpdateCount()
    {
        CountVisits +=1;
    }
}