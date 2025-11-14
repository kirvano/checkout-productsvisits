using Checkout.ProductsVisits.Shared.Result;
using Cortex.Mediator.Commands;

namespace Checkout.ProductsVisits.Application.UseCases.CreateOrUpdateVisit;

public class VisitCommand : ICommand<Result<VisitOutput>>
{
    public long ProductId{ get; set; }
}