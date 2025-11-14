using Checkout.ProductsVisits.Domain.Aggregate;
using Checkout.ProductsVisits.Shared.Repository.Interface;

namespace Checkout.ProductsVisits.Domain.Repositories;

public interface IProductsVisitsRepository : IRepository<ProductVisit,long>
{
    Task<bool> UpdateProductCount(ProductVisit  productVisit);
    Task<ProductVisit?> GetByProductIdAndDay(long  productId, string day);
}