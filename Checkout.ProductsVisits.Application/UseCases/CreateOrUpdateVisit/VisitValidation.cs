using FluentValidation;

namespace Checkout.ProductsVisits.Application.UseCases.CreateOrUpdateVisit;

public class VisitValidation
    : AbstractValidator<VisitCommand>
{

    public VisitValidation()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Product Id must be greater than zero");
    }
}