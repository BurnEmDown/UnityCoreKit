namespace Core.UpdateManagers.Interfaces
{
    /// <summary>
    /// Interface for objects that want to receive centralized <c>FixedUpdate</c> callbacks.
    /// </summary>
    /// <remarks>
    /// Implementing this interface allows a class to be registered with the
    /// global <see cref="Core.UpdateManagers.FixedUpdateManager"/>, which calls
    /// <see cref="ObservedFixedUpdate"/> every Unity <c>FixedUpdate()</c> frame.
    ///
    /// This is useful when you want deterministic, physics-timed update logic
    /// without placing the logic inside a MonoBehaviour’s own <c>FixedUpdate</c>,
    /// enabling centralized control over execution order and observer management.
    /// </remarks>
    public interface IFixedUpdateObserver
    {
        /// <summary>
        /// Called once per Unity <c>FixedUpdate</c> frame by the
        /// <see cref="Core.UpdateManagers.FixedUpdateManager"/>.
        /// </summary>
        /// <remarks>
        /// Use this method for physics-related or framerate-independent logic.
        /// Implementations should avoid long-running operations to prevent
        /// delaying the global update cycle.
        /// </remarks>
        void ObservedFixedUpdate();
    }
}