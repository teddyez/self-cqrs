# SelfCQRS

A lightweight CQRS implementation for .NET applications, similar to MediatR.

## Overview

SelfCQRS is a simple library that implements the CQRS (Command Query Responsibility Segregation) pattern in .NET applications. It provides a mediator pattern implementation that helps decouple senders and receivers of requests.

## Features

- Simple command and query handling
- Mediator pattern implementation
- Dependency injection support
- Lightweight and easy to use
- Similar API to MediatR

## Installation

```bash
dotnet add package SelfCQRS
```

## Usage

### 1. Define Commands and Queries

```csharp
public class CreateUserCommand : ICommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class GetUserQuery : IQuery<User>
{
    public int UserId { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

### 2. Implement Handlers

```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Implementation to create user
        await Task.CompletedTask;
    }
}

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
{
    public async Task<User> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        // Implementation to get user
        return await Task.FromResult(new User { Id = query.UserId, Name = "John Doe", Email = "john@example.com" });
    }
}
```

### 3. Register Services

In your `Program.cs` or `Startup.cs`:

```csharp
using SelfCQRS.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add SelfCQRS services
builder.Services.AddSelfCQRS()
    .AddHandlersFromThisAssembly();

// Or register handlers from a specific assembly
// builder.Services.AddSelfCQRS()
//     .AddHandlersFromAssembly(typeof(SomeHandler).Assembly);
```

### 4. Use the Mediator

```csharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var query = new GetUserQuery { UserId = id };
        var user = await _mediator.Send<GetUserQuery, User>(query);
        return Ok(user);
    }
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
