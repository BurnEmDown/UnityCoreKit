using Core.UpdateManagers.Interfaces;
using UnityEngine;

namespace Core.UpdateManagers
{
    /// <summary>
    /// Extension methods for registering MonoBehaviours to centralized update managers.
    /// </summary>
    public static class UpdateObserverExtensions
    {
        /// <summary>
        /// Registers a behaviour implementing <see cref="IUpdateObserver"/> with
        /// the global <see cref="UpdateManager"/> so it receives update ticks.
        /// </summary>
        public static T RegisterToUpdate<T>(this T behaviour)
            where T : MonoBehaviour, IUpdateObserver
        {
            UpdateManager.RegisterObserver(behaviour);
            return behaviour;
        }

        /// <summary>
        /// Unregisters a behaviour from the global <see cref="UpdateManager"/>.
        /// </summary>
        public static T UnregisterFromUpdate<T>(this T behaviour)
            where T : MonoBehaviour, IUpdateObserver
        {
            UpdateManager.UnregisterObserver(behaviour);
            return behaviour;
        }

        /// <summary>
        /// Registers a behaviour implementing <see cref="ILateUpdateObserver"/> with
        /// the global <see cref="LateUpdateManager"/>.
        /// </summary>
        public static T RegisterToLateUpdate<T>(this T behaviour)
            where T : MonoBehaviour, ILateUpdateObserver
        {
            LateUpdateManager.RegisterObserver(behaviour);
            return behaviour;
        }

        /// <summary>
        /// Registers a behaviour implementing <see cref="IFixedUpdateObserver"/> with
        /// the global <see cref="FixedUpdateManager"/>.
        /// </summary>
        public static T RegisterToFixedUpdate<T>(this T behaviour)
            where T : MonoBehaviour, IFixedUpdateObserver
        {
            FixedUpdateManager.RegisterObserver(behaviour);
            return behaviour;
        }
    }
}