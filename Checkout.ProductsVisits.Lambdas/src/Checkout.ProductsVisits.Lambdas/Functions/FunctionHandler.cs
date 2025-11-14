using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Checkout.ProductsVisits.Application.UseCases.CreateOrUpdateVisit;
using Checkout.ProductsVisits.Shared.Result;
using Cortex.Mediator;
using Microsoft.Extensions.Logging;


namespace Checkout.ProductsVisits.Lambdas.Functions;

public class FunctionHandler
{
    private readonly ILogger<FunctionHandler> _logger;
    private readonly IMediator _mediator;

    public FunctionHandler(ILogger<FunctionHandler> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="dynamoEvent">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<bool> HandleAsync(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Recebidos {dynamoEvent.Records.Count} registros do DynamoDB Stream.");

        foreach (var record in dynamoEvent.Records)
        {
            if (record.EventName == "INSERT")
            {
                var newItem = record.Dynamodb.NewImage;
                if (newItem == null)
                {
                    context.Logger.LogInformation("INSERT sem conteúdo de NewImage.");
                    continue;
                }

                var id = newItem.TryGetValue("productId", out DynamoDBEvent.AttributeValue value) ? value.S : "sem id";

                if (id.Equals("sem id"))
                    continue;
                
                var command = new VisitCommand
                {
                    ProductId = long.Parse(id)
                };
                					
                var result =
                    await _mediator.SendCommandAsync<VisitCommand, Result<VisitOutput>>(command);
                if (result.IsSuccess)
                {
                    _logger.LogDebug("Command executed successfully for message: {RecordMessageId} - response: {Output}",
                        record.EventID, JsonSerializer.Serialize(result.Value));
                    return true;
                }
                
                var error = result.Error;
                _logger.LogWarning("Error processing message {RecordMessageId}: {ErrorCode} - {ErrorMessage}",
                    record.EventID, error.Code, error.Message);
                return false;
            }
          
        }
        return true;
    }
}