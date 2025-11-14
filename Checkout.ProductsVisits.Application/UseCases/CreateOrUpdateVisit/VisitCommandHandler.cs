using Checkout.ProductsVisits.Domain.Aggregate;
using Checkout.ProductsVisits.Domain.Repositories;
using Checkout.ProductsVisits.Shared.Result;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Checkout.ProductsVisits.Application.UseCases.CreateOrUpdateVisit;

public class VisitCommandHandler(
    IProductsVisitsRepository productsVisitsRepository,
    IValidator<VisitCommand> visitCommandValidator,
    ILogger<VisitCommandHandler> logger)
    : ICommandHandler<VisitCommand, Result<VisitOutput>>
{
    public async Task<Result<VisitOutput>> Handle(VisitCommand command, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating a new product visit {ProductId}", command.ProductId);
            var validationResult = await visitCommandValidator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                return Result<VisitOutput>.Failure(Error.Failure("ValidationError",
                    validationResult.Errors.First().ErrorMessage));

            var day = DateTime.UtcNow;

            var teste = DateOnly.FromDateTime(day);
            
            var visit = await productsVisitsRepository.GetByProductIdAndDay(command.ProductId,
                teste.ToString());


            if (visit != null)
            {
                visit.UpdateCount();
                var result = await productsVisitsRepository.UpdateProductCount(visit);
                if (!result)
                    Error.Failure("ERROR_PRODUCT_VISIT", "Error changing product visit.");
            }
            else
            {
                visit = ProductVisit.Create(command.ProductId);
                await productsVisitsRepository.Upsert(visit, cancellationToken);
            }

            return new VisitOutput
            {
                ProductId = command.ProductId,
                Message = "Product visit created or updated",
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error when creating product visit {ProductId}", command.ProductId);
            return Error.Failure("UNEXPECTED_PRODUCT_VISIT", "Error creating product visit ");
        }
    }
}