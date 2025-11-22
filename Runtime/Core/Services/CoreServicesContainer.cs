using System;
using Core.Interfaces;

namespace Core.Services
{
    /// <summary>
    /// Provides an instance-based wrapper (facade) around the static <see cref="CoreServices"/> container.
    /// </summary>
    /// <remarks>
    /// This class is useful when systems require dependency injection through an
    /// <see cref="IServices"/> interface rather than directly referencing the static
    /// <see cref="CoreServices"/> class.
    ///
    /// It enables:
    /// <list type="bullet">
    ///     <item><description>Unit testing with mock service containers</description></item>
    ///     <item><description>Passing service locators into constructors</description></item>
    ///     <item><description>Better separation of concerns than direct static access</description></item>
    /// </list>
    ///
    /// All method calls delegate to the underlying static container to ensure a single,
    /// consistent source of truth for registered services.
    /// </remarks>
    public class ServicesContainer : IServices
    {
        /// <summary>
        /// Registers a concrete service instance in the global service container.
        /// </summary>
        /// <typeparam name="T">The service interface or type to register.</typeparam>
        /// <param name="instance">The actual service instance.</param>
        public void Register<T>(T instance) => CoreServices.Register(instance);

        /// <summary>
        /// Registers a factory function that lazily constructs the service on first request.
        /// </summary>
        /// <typeparam name="T">The service interface or type to register.</typeparam>
        /// <param name="factory">A factory method that returns a new instance of the service.</param>
        public void Register<T>(Func<T> factory) => CoreServices.Register(factory);

        /// <summary>
        /// Retrieves a service of type <typeparamref name="T"/> from the global container.
        /// </summary>
        /// <typeparam name="T">The service interface or type to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the requested service is not registered.
        /// </exception>
        public T Get<T>() => CoreServices.Get<T>();

        /// <summary>
        /// Attempts to resolve a service of type <typeparamref name="T"/> from the global container
        /// without throwing an exception if the service is not registered.
        /// </summary>
        /// <typeparam name="T">The service interface or type to resolve.</typeparam>
        /// <param name="service">The resolved service instance if available; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if the service is available; otherwise <c>false</c>.</returns>
        public bool TryGet<T>(out T service) => CoreServices.TryGet(out service);
    }
}