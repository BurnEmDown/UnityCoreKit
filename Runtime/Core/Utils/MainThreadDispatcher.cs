using System;
using System.Collections.Concurrent;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;

namespace Core.Utils
{
    /// <summary>
    /// A lightweight dispatcher used to marshal work from background threads
    /// onto Unity’s main thread.
    /// </summary>
    /// <remarks>
    /// Unity APIs (such as instantiating GameObjects, modifying Transforms,
    /// loading scenes, etc.) must be executed on the main thread.
    /// 
    /// <para>
    /// <see cref="MainThreadDispatcher"/> provides a globally accessible,
    /// thread-safe queue that any thread can enqueue actions into, and these
    /// actions will be flushed and executed on the main thread during
    /// <see cref="Update"/>.
    /// </para>
    ///
    /// <para>
    /// This class is automatically created and initialized once per application,
    /// using <see cref="RuntimeInitializeOnLoadMethodAttribute"/> so you never
    /// need to manually add it to a scene.
    /// </para>
    ///
    /// Typical usage:
    /// <code>
    /// Task.Run(() =>
    /// {
    ///     // background thread work
    ///     MainThreadDispatcher.Enqueue(() =>
    ///     {
    ///         // safe to call Unity API here
    ///         myGameObject.SetActive(true);
    ///     });
    /// });
    /// </code>
    /// </remarks>
    public class MainThreadDispatcher : MonoBehaviour
    {
        /// <summary>
        /// Thread-safe queue storing actions to be executed on the main thread.
        /// </summary>
        private static readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        /// <summary>
        /// Ensures the dispatcher is only initialized once.
        /// </summary>
        private static bool _initialized;

        /// <summary>
        /// Automatically creates the dispatcher before the first scene loads.
        /// Ensures the dispatcher persists across scenes.
        /// </summary>
        /// <remarks>
        /// This method runs before any scenes are loaded, ensuring the dispatcher
        /// is available even if async operations start very early in the lifecycle.
        /// </remarks>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_initialized) return;

            var go = new GameObject("[MainThreadDispatcher]");
            DontDestroyOnLoad(go);
            go.AddComponent<MainThreadDispatcher>();

            _initialized = true;
        }

        /// <summary>
        /// Enqueues an action to run on the Unity main thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <remarks>
        /// The action will be invoked during the next <see cref="Update"/> call.
        /// <para>
        /// This method is thread-safe and may be called from any thread,
        /// including worker threads, async continuations, and network callbacks.
        /// </para>
        /// </remarks>
        public static void Enqueue(Action action)
        {
            if (action == null) return;
            _queue.Enqueue(action);
        }

        /// <summary>
        /// Executes all queued main-thread actions.
        /// </summary>
        /// <remarks>
        /// All actions are executed in FIFO order.
        ///
        /// <para>
        /// Any exception thrown by an enqueued action is caught and logged using
        /// <see cref="Debug.LogException"/> to avoid breaking the dispatcher.
        /// </para>
        /// </remarks>
        private void Update()
        {
            while (_queue.TryDequeue(out var action))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
            }
        }
    }
}