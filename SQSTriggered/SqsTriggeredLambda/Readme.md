# AWS Lambda Simple SQS Function Project

Amazon SQS offers hosted queues that integrate and decouple distributed software systems and components. Amazon SQS provides a generic web services API that you can access using any programming language supported by AWS SDK. Messages in the queue are typically processed by a single subscriber. Amazon SQS and Amazon SNS are often used together to create a fanout messaging application.

SQS is a pull-based delivery system, which means that the consumer of the messages is responsible for pulling messages from the queue and processing them. In contrast, SNS is a push-based delivery system, which means that the service automatically pushes messages to the subscribed endpoints.

SQS is ideal for processing large numbers of messages in a reliable and scalable manner, making it a good choice for background tasks, batch jobs, and other types of workloads that can be parallelized. SNS, on the other hand, is better suited for time-critical, high-throughput workloads that require immediate processing of messages, such as mobile push notifications or real-time alerts.

One common use case for SQS and SNS is to build a serverless event-driven architecture, where events trigger the execution of code in AWS Lambda.

For example, you can use SQS to buffer incoming events, and then use SNS to push those events to one or more Lambda functions for processing. This allows you to scale your application horizontally by adding more Lambda functions, without having to worry about managing the underlying infrastructure.

## Advantages of having SNS => SQS => Lambda

1. SNS delivers the messages asynchronously, So the lambda has to take care of the failures and retries. It is managed using the InternalEventQueue.
2.  If we delete the Lambda then the messages will be lost after a few retries. So we add SQS as an intermediate, which acts as a buffer and the subscriber can read messages from it when it is ready again.
3.  We can deliver messages in batches to the lambda when using an SQS queue
4.  We can control the concurrency in the lambda and set a max number of instances as well.






































## References
* Basic Architecture (https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-basic-architecture.html)
* Rahul Nath's Blod - https://www.rahulpnath.com/blog/amazon-sns-to-lambda-or-sns-sqs-lambda-dotnet/#disadvantages-of-directly-processing-messages-from-sns-lambda
