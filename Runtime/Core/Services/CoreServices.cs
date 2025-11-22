using System;
using System.Collections.Generic;

namespace Core.Services
{
    /// <summary>
    /// Simple service locator / DI container for core services.
    /// Supports both direct instance registration and lazy factory registration.
    /// </summary>
    public static class CoreServices
    {
        /// <summary>
        /// Stores created service instances keyed by their service type.
        /// </summary>
        private static readonly Dictionary<Type, object> services = new();

        /// <summary>
        /// Stores service factories keyed by their service type.
        /// Factories are invoked lazily the first time the service is requested.
        /// </summary>
        private static readonly Dictionary<Type, Func<object>> factories = new();

        /// <summary>
        /// Registers a concrete service instance for the given type <typeparamref name="T"/>.
        /// Any previously registered factory for this type is removed.
        /// </summary>
        public static void Register<T>(T serviceInstance)
        {
            if (serviceInstance == null)
                throw new ArgumentNullException(nameof(serviceInstance));

            var type = typeof(T);
            services[type] = serviceInstance;
            factories.Remove(type);
        }

        /// <summary>
        /// Registers a factory for lazily creating a service of type <typeparamref name="T"/>.
        /// The factory is called the first time <see cref="Get{T}"/> is used for this type,
        /// and the resulting instance is cached for future calls.
        /// </summary>
        public static void Register<T>(Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var type = typeof(T);
            factories[type] = () => factory();
            services.Remove(type);
        }

        /// <summary>
        /// Resolves a service of type <typeparamref name="T"/>.
        /// If the service was registered using a factory, it is instantiated on first request.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no instance or factory has been registered for the requested type.
        /// </exception>
        public static T Get<T>()
        {
            var type = typeof(T);

            // If we already have an instance, return it.
            if (services.TryGetValue(type, out var instance))
            {
                return (T)instance;
            }

            // If we have a factory, create the instance, cache it, then return it.
            if (factories.TryGetValue(type, out var factory))
            {
                var created = (T)factory();
                if (created == null)
                    throw new InvalidOperationException($"Factory for type {type} returned null.");

                services[type] = created;
                return created;
            }

            throw new InvalidOperationException($"Service of type {type} is not registered.");
        }

        /// <summary>
        /// Attempts to resolve a service of type <typeparamref name="T"/> without throwing.
        /// </summary>
        /// <param name="service">The resolved service instance, or <c>null</c> if unavailable.</param>
        /// <returns>
        /// <c>true</c> if the service exists or can be constructed;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool TryGet<T>(out T service)
        {
            var type = typeof(T);

            // If instance exists → return it
            if (services.TryGetValue(type, out var instanceObj))
            {
                service = (T)instanceObj;
                return true;
            }

            // If factory exists → create it, cache it, return it
            if (factories.TryGetValue(type, out var factory))
            {
                var created = (T)factory();
                if (created == null)
                {
                    service = default;
                    return false;
                }

                services[type] = created;
                service = created;
                return true;
            }

            service = default;
            return false;
        }
    }
}