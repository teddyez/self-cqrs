using System;
using System.Threading;
using System.Threading.Tasks;

namespace SelfCQRS.Messaging;

/// <summary>
/// Implementation of the mediator pattern.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends the specified command to its handler.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"No handler registered for command '{command.GetType().FullName}'");

        var method = handler.GetType().GetMethod("Handle");
        if (method == null)
            throw new InvalidOperationException($"Handler for command '{command.GetType().FullName}' does not contain a Handle method");

        var task = (Task?)method.Invoke(handler, new object?[] { command, cancellationToken });
        if (task == null)
            throw new InvalidOperationException($"Handler for command '{command.GetType().FullName}' did not return a valid task");
            
        await task;
    }

    /// <summary>
    /// Sends the specified query to its handler.
    /// </summary>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the query result.</returns>
    public async Task<TResponse> Send<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"No handler registered for query '{query.GetType().FullName}'");

        var method = handler.GetType().GetMethod("Handle");
        if (method == null)
            throw new InvalidOperationException($"Handler for query '{query.GetType().FullName}' does not contain a Handle method");

        var task = (Task<TResponse>?)method.Invoke(handler, new object?[] { query, cancellationToken });
        if (task == null)
            throw new InvalidOperationException($"Handler for query '{query.GetType().FullName}' did not return a valid task");
            
        return await task;
    }
}