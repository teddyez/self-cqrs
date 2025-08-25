using Microsoft.Extensions.DependencyInjection;
using SelfCQRS.DependencyInjection;
using SelfCQRS.Messaging;

// Define a command
public class CreateUserCommand : ICommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Define a query
public class GetUserQuery : IQuery<User>
{
    public int UserId { get; set; }
}

// Define a result model
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Implement a command handler
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Creating user: {command.Name} ({command.Email})");
        await Task.CompletedTask;
    }
}

// Implement a query handler
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
{
    public async Task<User> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Getting user with ID: {query.UserId}");
        return await Task.FromResult(new User { Id = query.UserId, Name = "John Doe", Email = "john@example.com" });
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        // Set up dependency injection
        var services = new ServiceCollection();
        
        services.AddSelfCQRS()
                .AddHandlersFromThisAssembly();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Get the mediator
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
    }
}
