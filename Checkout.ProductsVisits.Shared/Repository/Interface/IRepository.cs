namespace Checkout.ProductsVisits.Shared.Repository.Interface;

public interface IRepository<TAggregate, TKey>
    where TAggregate : class
    where TKey : struct
{
    Task<bool> Exists(TKey id, CancellationToken cancellationToken = default);
    Task<TAggregate> Get(TKey id, CancellationToken cancellationToken = default);
    Task Upsert(TAggregate entity, CancellationToken cancellationToken = default);
}