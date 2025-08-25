namespace SelfCQRS.Messaging;

/// <summary>
/// Marker interface for queries.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the query.</typeparam>
public interface IQuery<out TResponse>
{
}