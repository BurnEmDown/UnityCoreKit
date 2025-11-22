using UnityEngine;

namespace Core.Utils.Extensions
{
    /// <summary>
    /// Utility extension methods for working with <see cref="GameObject"/> instances.
    /// </summary>
    /// <remarks>
    /// These helpers are intentionally dependency-free (beyond Unity itself)
    /// so that they can be reused across all layers of the project without
    /// introducing circular references.
    /// </remarks>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Retrieves a component of type <typeparamref name="T"/> from the GameObject.
        /// If the component does not exist, a new one is added and returned.
        /// </summary>
        /// <typeparam name="T">The component type to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to operate on.</param>
        /// <returns>The existing or newly created component.</returns>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}