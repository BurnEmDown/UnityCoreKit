namespace Core.UpdateManagers.Interfaces
{
    /// <summary>
    /// Interface for objects that want to receive centralized <c>Update</c> callbacks.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive update ticks from the global
    /// <see cref="Core.UpdateManagers.UpdateManager"/>, which invokes
    /// <see cref="ObservedUpdate"/> once per Unity <c>Update()</c> frame.
    ///
    /// This allows you to avoid scattering multiple MonoBehaviour <c>Update</c>
    /// methods across your project and instead route all per-frame logic
    /// through a deterministic, centralized system.
    /// </remarks>
    public interface IUpdateObserver
    {
        /// <summary>
        /// Called once per Unity <c>Update</c> frame by the
        /// <see cref="Core.UpdateManagers.UpdateManager"/>.
        /// </summary>
        /// <remarks>
        /// Use this for general per-frame logic, movement, timers, state
        /// machines, and anything that depends on <c>Time.deltaTime</c>.
        ///
        /// Avoid expensive operations inside this method, as all observers
        /// share the same update pass.
        /// </remarks>
        void ObservedUpdate();
    }
}