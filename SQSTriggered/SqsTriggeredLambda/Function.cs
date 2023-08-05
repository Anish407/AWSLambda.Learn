using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SqsTriggeredLambda;

public class Function
{
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {

    }


    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<SQSBatchResponse> FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        // every time a message in a batch fails, the entire batch will be resent 
        // To avoid this scenario, we need to let the lambda know about the particular message
        // in the batch that failed, this is done using the SQSBatchResponse class
        SQSBatchResponse response = new SQSBatchResponse
        {
            BatchItemFailures = new List<SQSBatchResponse.BatchItemFailure>()
        };
        context.Logger.LogInformation(evnt.Records.Count.ToString());
        foreach (var message in evnt.Records)
        {
            try
            {
                await ProcessMessageAsync(message, context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex.Message);
                // the lambda will retry this particular message and not the entire batch
                response.BatchItemFailures.Add(new SQSBatchResponse.BatchItemFailure()
                {
                    ItemIdentifier= message.MessageId
                });
            }
        }
        return response;
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed message {message.Body}");

        // TODO: Do interesting work based on the new message
        await Task.CompletedTask;
    }
}