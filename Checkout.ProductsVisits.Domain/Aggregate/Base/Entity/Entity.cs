namespace Checkout.ProductsVisits.Domain.Aggregate.Base.Entity;

public abstract class Entity<TKey> where TKey : struct
{
    public TKey Id { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(TKey id)
    {
        Id = id;
    }

    public void SetId(TKey id)
    {
        if (Type.GetTypeCode(typeof(TKey)) is not (TypeCode.Int32 or TypeCode.Int64))
        {
            return;
        }

        if (Id is 0)
            Id = id;
    }
}