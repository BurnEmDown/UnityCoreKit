namespace Core.UpdateManagers.Interfaces
{
    /// <summary>
    /// Interface for objects that want to receive centralized <c>LateUpdate</c> callbacks.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive calls from the global
    /// <see cref="Core.UpdateManagers.LateUpdateManager"/>, which invokes
    /// <see cref="ObservedLateUpdate"/> once per Unity <c>LateUpdate()</c> frame.
    ///
    /// This hook is ideal for camera adjustments, cleanup logic, and actions that
    /// must occur after all <c>Update()</c> methods have finished running.
    /// </remarks>
    public interface ILateUpdateObserver
    {
        /// <summary>
        /// Called once per Unity <c>LateUpdate</c> frame by the
        /// <see cref="Core.UpdateManagers.LateUpdateManager"/>.
        /// </summary>
        /// <remarks>
        /// Use this for tasks that should always happen after normal update logic,
        /// such as camera following, smoothing, or state cleanup.
        /// </remarks>
        void ObservedLateUpdate();
    }
}