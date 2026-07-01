# TopUpSimulation
 
A **.NET 8 Worker Service** that simulates an event-driven mobile top-up (recharge) processing pipeline. It implements the **Transactional Outbox pattern** to reliably charge a mobile top-up provider, persist the resulting transaction, and publish integration events over **Apache Kafka** — all wired together with a pragmatic **Clean Architecture** layout.
 
<p align="left">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white">
  <img alt="C%23" src="https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white">
  <img alt="Kafka" src="https://img.shields.io/badge/Kafka-Confluent.Kafka-231F20?logo=apachekafka&logoColor=white">
  <img alt="EF Core" src="https://img.shields.io/badge/EF%20Core-InMemory-512BD4">
  <img alt="Serilog" src="https://img.shields.io/badge/Logging-Serilog-1F4B99">
  <img alt="License" src="https://img.shields.io/badge/License-MIT-green">
</p>
---
 
## Table of Contents
 
- [Overview](#overview)
- [Architecture](#architecture)
- [How it Works](#how-it-works)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Configuration](#configuration)
  - [Running the Worker](#running-the-worker)
  - [Running Tests](#running-tests)
- [Domain Model](#domain-model)
- [Roadmap Ideas](#roadmap-ideas)
- [License](#license)
---
 
## Overview
 
`TopUpSimulation` models a background worker for a mobile top-up (prepaid recharge) flow, inspired by real-world payment/telecom charging systems (e.g. MCI). Instead of charging a customer synchronously on request, charge requests are written to an **outbox table**. A background worker continuously polls the outbox, calls the top-up provider, records the outcome as a `Transaction`, and publishes a domain event to Kafka — guaranteeing at-least-once processing and decoupling the charging side-effect from the initial request.
 
A second background service consumes upstream Kafka events (e.g. `ShaparakTransactionWaitingForConfirmEvent`) and dispatches them to in-process handlers, closing the loop between payment-gateway confirmation and top-up execution.
 
## Architecture
 
The solution follows a layered / Clean Architecture style, split across four solution folders:
 
| Layer | Projects | Responsibility |
|---|---|---|
| **Presentation** | `TopUpSimulation.Worker` | Host/entry point (`Generic Host`), composition root, `BackgroundService` |
| **Application** | `TopUpSimulation.Handlers`, `TopUpSimulation.Handlers.Contracts` | Use-case orchestration, Kafka producer/consumer wiring, event dispatching, service contracts (`ITopUpService`) |
| **Core (Domain)** | `TopUpSimulation.Domain`, `TopUpSimulation.Resources` | Entities, domain events, repository contracts — no external dependencies |
| **Infrastructure** | `TopUpSimulation.Persistence`, `TopUpSimulation.ExternalServices` | EF Core persistence (Outbox/Transaction repositories), MCI top-up HTTP client |
| **Framework** | `Framework.Common`, `Framework.Core`, `Framework.Infrastructure`, `Framework.Logger`, `Framework.Presentation` | Cross-cutting building blocks: base `Entity`, `IUnitOfWork`/`IRepository`, event abstractions, settings, Serilog setup, exception middleware |
 
Dependencies flow inward — `Presentation → Application → Domain`, with `Infrastructure` and `Framework` supplying implementations through dependency injection, so the domain stays free of framework concerns.
 
<p align="center">
  <img src="docs/Architecture%20Diagram.png" alt="Architecture Diagram" width="800">
</p>
## How it Works
 
<p align="center">
  <img src="docs/FlowChart%20Diagram.png" alt="Flow Chart Diagram" width="800">
</p>
The worker runs two independent background loops (registered via `AddHostedService`):
 
**1. Outbox Processor (`Worker.cs`)**
1. Polls `TopUpOutBox` for all records where `IsProcessed == false`.
2. Deserializes the stored `InstantChargeRequest` payload.
3. Calls `ITopUpService.InstantCharge(...)` (implemented by `MCITopUpService`) to charge the subscriber.
4. Persists the outcome as a new `Transaction` (request, response, success flag).
5. Publishes a `TopUpRespondedEvent` to Kafka via `IEventPublisher`.
6. Marks the outbox entry `ProcessedCompleted()` and commits everything through `IUnitOfWork`.
7. Failures on an individual event are logged and skipped, so one bad message never blocks the rest of the batch.
**2. Kafka Consumer (`KafkaConsumerService.cs`)**
1. Subscribes to inbound topics (e.g. `ShaparakTransactionWaitingForConfirmEvent`).
2. Hands each message to `EventDispatcher`, which deserializes it and routes it to the matching `IEventHandler<T>` implementations via DI.
This design keeps the "call an external, unreliable API" step idempotent and retriable, and keeps producing/consuming Kafka events decoupled from the core charging logic.
 
## Project Structure
 
```
TopUpSimulation/
├── src/
│   ├── Presentation/
│   │   └── TopUpSimulation.Worker/          # Host, DI composition, BackgroundService
│   ├── Application/
│   │   ├── TopUpSimulation.Handlers/         # Kafka producer/consumer, dispatcher, DI extensions
│   │   └── TopUpSimulation.Handlers.Contracts/ # ITopUpService + request/response DTOs
│   ├── Core/
│   │   ├── TopUpSimulation.Domain/           # Entities, domain events, repository contracts
│   │   └── TopUpSimulation.Resources/        # Shared resources
│   ├── Infrastructure/
│   │   ├── TopUpSimulation.Persistence/      # EF Core DbContext + repositories
│   │   └── TopUpSimulation.ExternalServices/ # MCI top-up REST client
│   └── Framework/
│       ├── TopUpSimulation.Framework.Common/         # Settings, exceptions, RestfulClient
│       ├── TopUpSimulation.Framework.Core/           # Entity base class, Unit of Work, event abstractions
│       ├── TopUpSimulation.Framework.Infrastructure/ # UnitOfWork / Repository implementations
│       ├── TopUpSimulation.Framework.Logger/         # Serilog configuration
│       └── TopUpSimulation.Framework.Presentation/   # Exception middleware
├── tests/
│   └── TopUpSimulation.Tests/                # xUnit test project
├── docs/
│   ├── Architecture Diagram.png
│   └── FlowChart Diagram.png
├── TopUpSimulation.sln
└── LICENSE.txt
```
 
## Tech Stack
 
- **.NET 8** — Worker Service (`Microsoft.NET.Sdk.Worker`) hosted via the Generic Host
- **Confluent.Kafka** — event production and consumption
- **Entity Framework Core 8 (InMemory provider)** — persistence for `Transaction` and `TopUpOutBox`
- **Serilog** — structured logging to console and rolling daily log files
- **Newtonsoft.Json / System.Text.Json** — serialization
- **MediatR** — available in the core framework layer for in-process messaging
- **xUnit** — test project scaffold
## Getting Started
 
### Prerequisites
 
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A reachable Kafka broker (both the producer and consumer clients are created at startup). For local development, spin one up with Docker, for example:
```bash
  docker run -d --name kafka -p 9092:9092 apache/kafka:latest
```
 
### Configuration
 
Settings live in `src/Presentation/TopUpSimulation.Worker/appsettings.json`:
 
| Section | Key | Description |
|---|---|---|
| `InMemoryDbName` | — | Name of the EF Core in-memory database instance |
| `MCITopUpSettings` | `BaseUrl`, `InstantCharge`, `SecretToken` | Top-up provider endpoint and auth token |
| | `MockService` | When `true`, charge calls are short-circuited to always succeed — no real HTTP call is made |
| `KafkaSettings` | `ProducerConfigs.BootstrapServers` | Kafka brokers used for publishing events |
| | `ConsumerConfigs.BootstrapServers`, `GroupId` | Kafka brokers/group used for consuming events |
| `Serilog` | `WriteTo`, `MinimumLevel`, ... | Console + rolling file sinks (`logs/log-.txt`) |
 
By default `MockService` is `true`, so the worker can run end-to-end without a real top-up provider — but a Kafka broker is still required since the producer/consumer are constructed eagerly during startup.
 
### Running the Worker
 
```bash
git clone <repository-url>
cd TopUpSimulation
 
dotnet restore
dotnet build
 
dotnet run --project src/Presentation/TopUpSimulation.Worker/TopUpSimulation.Worker.csproj
```
 
The worker will start polling the outbox and log activity to both the console and `logs/log-.txt`.
 
### Running Tests
 
```bash
dotnet test tests/TopUpSimulation.Tests/TopUpSimulation.Tests.csproj
```
 
## Domain Model
 
- **`TopUpOutBox`** — an outbox record holding the serialized charge request, a correlation id, `OccurredOn`, and an `IsProcessed` flag that the worker flips once handled.
- **`Transaction`** — the persisted outcome of a charge attempt: original request, provider response, and a `Successful` flag.
- **`TopUpRespondedEvent`** — integration event published to Kafka once a transaction is finalized.
- **`ShaparakTransactionWaitingForConfirmEvent`** — inbound integration event consumed to trigger downstream processing.
## Roadmap Ideas
 
- Swap the EF Core InMemory provider for a durable store (SQL Server/PostgreSQL) for production use.
- Add retry/backoff and dead-letter handling around the outbox loop and Kafka consumer.
- Flesh out the `tests` project with real assertions covering the outbox, worker, and consumer flows.
- Add health checks and metrics/tracing (OpenTelemetry) around the Kafka and HTTP integrations.
## License
 
Distributed under the **MIT License**. See [`LICENSE.txt`](LICENSE.txt) for details.