namespace Core.Interfaces
{
    /// <summary>
    /// Provides access to Unity time values and controls for pausing and resuming gameplay time.
    /// </summary>
    /// <remarks>
    /// A time service abstracts Unity's <see cref="UnityEngine.Time"/> API, allowing systems
    /// to query delta times or manipulate the time scale without referencing Unity directly.
    /// It also enables easier mocking and testing.
    /// </remarks>
    public interface ITimeService
    {
        /// <summary>
        /// Gets the scaled delta time for the current frame, equivalent to <c>Time.deltaTime</c>.
        /// This value is affected by <see cref="TimeScale"/>.
        /// </summary>
        float DeltaTime { get; }

        /// <summary>
        /// Gets the unscaled delta time for the current frame, equivalent to <c>Time.unscaledDeltaTime</c>.
        /// This value is not affected by <see cref="TimeScale"/>.
        /// </summary>
        float UnscaledDeltaTime { get; }

        /// <summary>
        /// Gets or sets the global time scale, equivalent to <c>Time.timeScale</c>.
        /// A value of 1.0 represents normal time, while 0.0 represents a paused state.
        /// </summary>
        float TimeScale { get; set; }

        /// <summary>
        /// Pauses the game by setting <see cref="TimeScale"/> to 0.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the game from a paused state by restoring <see cref="TimeScale"/> to 1.
        /// </summary>
        void Resume();
    }
}