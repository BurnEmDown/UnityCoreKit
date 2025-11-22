using Core.Interfaces;
using UnityEngine;

namespace Core.DefaultServices
{
    /// <summary>
    /// Default implementation of <see cref="ITimeService"/> using Unity's <see cref="Time"/> API.
    /// </summary>
    /// <remarks>
    /// This service wraps Unity time values to allow systems to query delta times and control
    /// pause/resume without directly referencing <see cref="UnityEngine.Time"/>.
    /// 
    /// The service maintains its own cached time scale to restore the previous value when
    /// resuming from pause.
    /// </remarks>
    public class DefaultTimeService : ITimeService
    {
        private float _previousTimeScale = 1f;

        /// <summary>
        /// Gets the scaled delta time from <see cref="Time.deltaTime"/>.
        /// </summary>
        public float DeltaTime => Time.deltaTime;

        /// <summary>
        /// Gets the unscaled delta time from <see cref="Time.unscaledDeltaTime"/>.
        /// </summary>
        public float UnscaledDeltaTime => Time.unscaledDeltaTime;

        /// <summary>
        /// Gets or sets the global time scale, wrapping <see cref="Time.timeScale"/>.
        /// </summary>
        public float TimeScale
        {
            get => Time.timeScale;
            set => Time.timeScale = Mathf.Max(value, 0f); // Prevent negative timescales
        }

        /// <summary>
        /// Pauses gameplay by setting <see cref="TimeScale"/> to 0.
        /// </summary>
        public void Pause()
        {
            if (Time.timeScale > 0f)
            {
                _previousTimeScale = Time.timeScale;
            }

            Time.timeScale = 0f;
        }

        /// <summary>
        /// Resumes gameplay by restoring the last non-zero time scale.
        /// </summary>
        public void Resume()
        {
            // If something set timeScale manually while paused, ensure a sensible fallback
            if (_previousTimeScale <= 0f)
                _previousTimeScale = 1f;

            Time.timeScale = _previousTimeScale;
        }
    }
}
