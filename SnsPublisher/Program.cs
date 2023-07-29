// See https://aka.ms/new-console-template for more information
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;

var serviceProvider = ConfigureDI();

try
{
    IAmazonSimpleNotificationService snsClient = serviceProvider.GetRequiredService<IAmazonSimpleNotificationService>();
    //await PublishMessage(snsClient);
    await PublishBatchMessage(snsClient);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message); ;
}

Console.WriteLine("Hello, World!");


async Task PublishMessage(IAmazonSimpleNotificationService snsClient)
{
    var request = new PublishRequest
    {
        Message = "Message From Console app",
        TopicArn = ""
    };

    try
    {
        var response = await snsClient.PublishAsync(request);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending message: {ex}");
    }
}

async Task PublishBatchMessage(IAmazonSimpleNotificationService snsClient)
{

    var batchMessages = Enumerable.Range(0, 25).Select(i => new PublishBatchRequestEntry
    {
        Message = $"MessageNumber:{i} - Message from ConsoleApp",
        Id = i.ToString(),
        Subject = $"Subject:{i}"
    }).ToList();


    try
    {
        // Sns has a limit of 10 messages per batch, if we directly publish the messages,
        // we will get the following error
        // 'The batch request contains more entries than permissible.'

        // this code will error out if we publish more than 10 messages at once
        //var response = await snsClient.PublishBatchAsync(new PublishBatchRequest
        //{
        //    PublishBatchRequestEntries= batchMessages,
        //    TopicArn = "arn:aws:sns:eu-north-1:170454899704:SnslambdaTest"
        //});

        var batchResponse = await Task.WhenAll(batchMessages.Batch(10).Select(
             async batch => await snsClient.PublishBatchAsync(new PublishBatchRequest
             {
                 PublishBatchRequestEntries = batch.ToList(),
                 TopicArn = ""
             })));

        foreach (var item in batchResponse)
        {
             Console.Out.WriteLine(item.Successful); 
        }
        await Console.Out.WriteLineAsync("Published all messages");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending message: {ex}");
    }
}

static IServiceProvider ConfigureDI()
{
    return new ServiceCollection()
                          .AddAWSService<IAmazonSimpleNotificationService>()
                          .BuildServiceProvider();
}

public static class IEnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source, int size)
    {
        T[] bucket = null;
        var count = 0;

        foreach (var item in source)
        {
            if (bucket == null)
                bucket = new T[size];

            bucket[count++] = item;

            if (count != size)
                continue;

            yield return bucket.ToList();

            bucket = null;
            count = 0;
        }

        // Return the last bucket with all remaining elements
        if (bucket != null && count > 0)
        {
            Array.Resize(ref bucket, count);
            yield return bucket.Select(x => x);
        }
    }
}