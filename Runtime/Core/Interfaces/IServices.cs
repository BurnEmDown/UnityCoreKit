using System;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a minimal service container capable of registering and resolving
    /// application-wide service instances.
    /// </summary>
    /// <remarks>
    /// Implementations may act as a lightweight dependency-injection container.
    /// Supports both direct instance registration and lazy factory registration,
    /// enabling flexible initialization strategies.
    /// </remarks>
    public interface IServices
    {
        /// <summary>
        /// Registers a concrete service instance for the given type 
        /// <typeparamref name="T"/>. 
        /// Any previously registered factory for this type should be removed.
        /// </summary>
        /// <typeparam name="T">The service interface or type.</typeparam>
        /// <param name="instance">The concrete instance to register.</param>
        void Register<T>(T instance);

        /// <summary>
        /// Registers a factory for lazily creating a service of type 
        /// <typeparamref name="T"/>. 
        /// The factory is called the first time the service is resolved.
        /// </summary>
        /// <typeparam name="T">The service interface or type.</typeparam>
        /// <param name="factory">A function that constructs the service instance.</param>
        void Register<T>(Func<T> factory);

        /// <summary>
        /// Resolves a service of type <typeparamref name="T"/>. 
        /// If the service was registered with a factory, the factory should be
        /// invoked on first resolution and the result cached.
        /// </summary>
        /// <typeparam name="T">The service interface or type to resolve.</typeparam>
        /// <returns>The service instance associated with type <typeparamref name="T"/>.</returns>
        T Get<T>();

        /// <summary>
        /// Attempts to resolve a service of type <typeparamref name="T"/> without throwing.
        /// </summary>
        /// <typeparam name="T">The service interface or type.</typeparam>
        /// <param name="service">The resolved service instance, or <c>null</c> if unavailable.</param>
        /// <returns>
        /// <c>true</c> if the service exists or can be constructed; 
        /// <c>false</c> otherwise.
        /// </returns>
        bool TryGet<T>(out T service);
    }
}