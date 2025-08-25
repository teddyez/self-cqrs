namespace SelfCQRS.Messaging;

/// <summary>
/// Interface for the mediator.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends the specified command to its handler.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand;

    /// <summary>
    /// Sends the specified query to its handler.
    /// </summary>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the query result.</returns>
    Task<TResponse> Send<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : IQuery<TResponse>;
}