# PollyHangfire

[![NuGet](https://img.shields.io/nuget/v/PollyHangfire.svg)](https://www.nuget.org/packages/PollyHangfire/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PollyHangfire.svg)](https://www.nuget.org/packages/PollyHangfire/)
[![CI](https://github.com/Swevo/PollyHangfire/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/PollyHangfire/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Polly v8 resilience pipelines for Hangfire** — retry, timeout, and circuit-breaker for IBackgroundJobClient.Enqueue and Schedule.

## The gap Hangfire leaves open

Hangfire's built-in retry applies to job **execution** on the server side. It does **not** protect the **client side** — when your app calls Enqueue() or Schedule() and the backing job store (Redis, SQL Server, etc.) is temporarily unavailable, you get an unhandled BackgroundJobClientException or DistributedLockTimeoutException.

PollyHangfire fills that gap.

## Installation

`
dotnet add package PollyHangfire
`

## Quick start

### DI registration

`csharp
services.AddPollyHangfire(pipeline =>
    pipeline
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            BackoffType = DelayBackoffType.Exponential,
            ShouldHandle = HangfireTransientErrors.IsTransient,
        })
        .AddTimeout(TimeSpan.FromSeconds(10)));

// Requires IBackgroundJobClient to already be registered (e.g. via services.AddHangfire(...))
public class OrderService(ResilientBackgroundJobClient jobs)
{
    public void PlaceOrder(Order order)
        => jobs.Enqueue<IOrderProcessor>(p => p.ProcessAsync(order));
}
`

### WithPolly() extension on existing client

`csharp
IBackgroundJobClient client = new BackgroundJobClient();

var resilient = client.WithPolly(pipeline =>
    pipeline.AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(500),
        BackoffType = DelayBackoffType.Exponential,
        ShouldHandle = HangfireTransientErrors.IsTransient,
    }));

resilient.Enqueue<IEmailSender>(s => s.SendWelcomeEmailAsync(userId));
resilient.Schedule<IReportGenerator>(r => r.GenerateAsync(reportId), TimeSpan.FromHours(1));
`

## API surface

| Method | Description |
|---|---|
| Enqueue(Expression<Action>) | Enqueue fire-and-forget job |
| Enqueue<T>(Expression<Action<T>>) | Enqueue typed fire-and-forget job |
| Schedule(Expression<Action>, TimeSpan) | Schedule delayed job |
| Schedule<T>(Expression<Action<T>>, TimeSpan) | Schedule typed delayed job |
| Execute<T>(Func<T>) | Execute arbitrary function in pipeline |

## Transient error predicate

HangfireTransientErrors.IsTransient handles:

| Exception | Reason |
|---|---|
| BackgroundJobClientException | Job store temporarily unavailable |
| DistributedLockTimeoutException | Distributed lock acquisition failed |
| TimeoutException | Store connection or command timed out |
| TaskCanceledException | Operation cancelled |

## Supported frameworks


et6.0 · 
et8.0 · 
et9.0

## Related packages

| Package | Wraps |
|---|---|
| [PollyEFCore](https://github.com/Swevo/PollyEFCore) | Entity Framework Core DbContext |
| [PollyDapper](https://github.com/Swevo/PollyDapper) | Dapper IDbConnection |
| [PollyMongo](https://github.com/Swevo/PollyMongo) | MongoDB IMongoCollection<T> |
| [PollyAzureBlob](https://github.com/Swevo/PollyAzureBlob) | Azure Blob Storage BlobContainerClient |
| [PollyNpgsql](https://github.com/Swevo/PollyNpgsql) | Npgsql PostgreSQL NpgsqlConnection |
| [PollySqlClient](https://github.com/Swevo/PollySqlClient) | System.Data.SqlClient SqlConnection |
| [PollyCosmosDb](https://github.com/Swevo/PollyCosmosDb) | Azure Cosmos DB CosmosClient |
| [PollyGrpc](https://github.com/Swevo/PollyGrpc) | gRPC channel calls |
| [PollyRabbitMQ](https://github.com/Swevo/PollyRabbitMQ) | RabbitMQ IModel channel |
| [PollyAzureServiceBus](https://github.com/Swevo/PollyAzureServiceBus) | Azure Service Bus sender/receiver |
| [PollyRedis](https://github.com/Swevo/PollyRedis) | StackExchange.Redis IDatabase |
| [PollyMediatR](https://github.com/Swevo/PollyMediatR) | MediatR IMediator |
| [PollyOpenAI](https://github.com/Swevo/PollyOpenAI) | OpenAI ChatClient |
| [PollyHealthChecks](https://github.com/Swevo/PollyHealthChecks) | ASP.NET Core health checks |
| [PollyBackoff](https://github.com/Swevo/PollyBackoff) | Pre-built backoff pipelines |
| [PollyChaos](https://github.com/Swevo/PollyChaos) | Chaos engineering helpers |
| [PollyKafka](https://github.com/Swevo/PollyKafka) | Confluent Kafka producer/consumer |
| [PollySignalR](https://github.com/Swevo/PollySignalR) | SignalR HubConnection |
| [PollyRateLimiter](https://github.com/Swevo/PollyRateLimiter) | .NET rate limiting middleware |
| [PollyElasticsearch](https://github.com/Swevo/PollyElasticsearch) | Elastic.Clients.Elasticsearch |
| [PollyAzureKeyVault](https://github.com/Swevo/PollyAzureKeyVault) | Azure Key Vault SecretClient |
| [PollyAzureEventHub](https://github.com/Swevo/PollyAzureEventHub) | Azure Event Hubs producer |
| [PollySendGrid](https://github.com/Swevo/PollySendGrid) | SendGrid email client |
| [PollyMassTransit](https://github.com/Swevo/PollyMassTransit) | MassTransit IBus |
| [PollyAzureTableStorage](https://github.com/Swevo/PollyAzureTableStorage) | Azure Table Storage TableClient |
| [PollyMailKit](https://github.com/Swevo/PollyMailKit) | MailKit SMTP email client |
| [PollyAzureQueueStorage](https://github.com/Swevo/PollyAzureQueueStorage) | Azure Queue Storage QueueClient |

## 💼 Need .NET consulting?

The author of this package is available for consulting on **Polly v8 resilience**, **Azure cloud architecture**, and **clean .NET design**.

**[→ solidqualitysolutions.com](https://www.solidqualitysolutions.com/)**
## License

MIT © Justin Bannister