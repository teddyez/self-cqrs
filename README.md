# SelfCQRS

A lightweight CQRS implementation for .NET applications, similar to MediatR.

## Overview

SelfCQRS is a simple library that implements the CQRS (Command Query Responsibility Segregation) pattern in .NET. It provides a mediator pattern implementation that allows you to send commands and queries to their respective handlers without direct coupling.

## Features

- Lightweight implementation of CQRS pattern
- Mediator pattern for handling commands and queries
- Dependency injection support
- Easy to use and configure
- Similar API to MediatR for familiar usage

## Installation

To use SelfCQRS in your project, add a reference to the SelfCQRS project:

```xml
<ProjectReference Include="..\SelfCQRS\SelfCQRS.csproj" />
```

Or if packaged as a NuGet package:

```bash
dotnet add package SelfCQRS
```

## Usage

### 1. Register Services

In your `Program.cs` or startup configuration, register SelfCQRS services:

```csharp
var services = new ServiceCollection();
services.AddSelfCQRS()
        .AddHandlersFromThisAssembly();
```

### 2. Define Commands

Create command classes that implement `ICommand`:

```csharp
public class CreateUserCommand : ICommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}
```

### 3. Implement Command Handlers

Create handlers for your commands:

```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Handle the command
        Console.WriteLine($"Creating user: {command.Name} ({command.Email})");
        await Task.CompletedTask;
    }
}
```

### 4. Define Queries

Create query classes that implement `IQuery<TResponse>`:

```csharp
public class GetUserQuery : IQuery<User>
{
    public int UserId { get; set; }
}
```

### 5. Implement Query Handlers

Create handlers for your queries:

```csharp
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
{
    public async Task<User> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        // Handle the query
        return await Task.FromResult(new User { Id = query.UserId, Name = "John Doe", Email = "john@example.com" });
    }
}
```

### 6. Send Commands and Queries

Use the mediator to send commands and queries:

```csharp
// Get the mediator from the service provider
var mediator = serviceProvider.GetRequiredService<IMediator>();

// Send a command
var createUserCommand = new CreateUserCommand
{
    Name = "John Doe",
    Email = "john@example.com"
};

await mediator.Send(createUserCommand);

// Send a query
var getUserQuery = new GetUserQuery
{
    UserId = 1
};

var user = await mediator.Send<GetUserQuery, User>(getUserQuery);
Console.WriteLine($"Retrieved user: {user.Name} ({user.Email})");
```

## Project Structure

``` Markdown
SelfCQRS/
├── Messaging/
│   ├── ICommand.cs
│   ├── ICommandHandler.cs
│   ├── IQuery.cs
│   ├── IQueryHandler.cs
│   ├── IMediator.cs
│   └── Mediator.cs
├── DependencyInjection/
│   └── ServiceCollectionExtensions.cs
├── SelfCQRS.csproj
└── README.md

```

## Example

See the `SelfCQRS.Example` at [https://github.com/TeddyLab/SelfCQRS] project for a complete working example of how to use SelfCQRS.

## License

This project is licensed under the MIT License - see the [LICENSE](SelfCQRS/LICENSE) file for details.

## Author

Teddy

## Repository

[https://github.com/TeddyLab/SelfCQRS](https://github.com/TeddyLab/SelfCQRS)

**AI GENERATED**
