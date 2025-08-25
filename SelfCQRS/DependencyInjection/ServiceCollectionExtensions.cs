using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SelfCQRS.Messaging;

namespace SelfCQRS.DependencyInjection;

/// <summary>
/// Extension methods for setting up SelfCQRS services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SelfCQRS services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSelfCQRS(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddScoped<IMediator, Mediator>();
        return services;
    }

    /// <summary>
    /// Registers command and query handlers from the specified assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="assembly">The assembly to scan for handlers.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && IsHandler(t));

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterface = handlerType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && (
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

            if (handlerInterface != null)
            {
                services.AddScoped(handlerInterface, handlerType);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers command and query handlers from the calling assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddHandlersFromThisAssembly(this IServiceCollection services)
    {
        return services.AddHandlersFromAssembly(Assembly.GetCallingAssembly());
    }

    private static bool IsHandler(Type type)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && (
            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));
    }
}