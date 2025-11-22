using System;
using System.Threading.Tasks;

namespace Core.Utils.Extensions
{
    /// <summary>
    /// Extension helpers for scheduling Task continuations on Unity’s main thread.
    /// </summary>
    /// <remarks>
    /// Unity APIs (instantiating GameObjects, modifying transforms, etc.) must be
    /// executed on the main thread.  
    /// These helpers allow you to await a <see cref="Task"/> on a background thread
    /// and then enqueue a continuation back onto Unity's main thread using
    /// <see cref="MainThreadDispatcher"/>.
    ///
    /// <para>
    /// This mirrors Firebase’s <c>ContinueWithOnMainThread</c> pattern but keeps
    /// your architecture platform-agnostic.
    /// </para>
    /// </remarks>
    public static class TaskExtensions
    {
        /// <summary>
        /// Schedules a continuation to run on Unity’s main thread
        /// after the given <see cref="Task{T}"/> completes.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The task to continue from.</param>
        /// <param name="continuation">
        /// The callback to execute once the task is finished, executed on the main thread.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="task"/> or <paramref name="continuation"/> is null.
        /// </exception>
        /// <remarks>
        /// The continuation receives the completed task so it can inspect:
        /// <list type="bullet">
        ///     <item><description><c>task.Result</c></description></item>
        ///     <item><description><c>task.Exception</c></description></item>
        ///     <item><description><c>task.IsCanceled</c></description></item>
        /// </list>
        ///
        /// This method does *not* swallow exceptions — if the continuation throws, it
        /// will still surface in Unity’s console.
        /// </remarks>
        public static void ContinueWithOnMainThread<T>(
            this Task<T> task,
            Action<Task<T>> continuation)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuation == null) throw new ArgumentNullException(nameof(continuation));

            task.ContinueWith(t =>
            {
                MainThreadDispatcher.Enqueue(() => continuation(t));
            });
        }

        /// <summary>
        /// Schedules a continuation to run on Unity’s main thread
        /// after a non-generic <see cref="Task"/> completes.
        /// </summary>
        /// <param name="task">The task to continue from.</param>
        /// <param name="continuation">
        /// The callback to execute once the task is finished, executed on the main thread.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="task"/> or <paramref name="continuation"/> is null.
        /// </exception>
        /// <remarks>
        /// Use this overload for fire-and-forget async operations or tasks that
        /// do not return a result.
        /// </remarks>
        public static void ContinueWithOnMainThread(
            this Task task,
            Action<Task> continuation)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuation == null) throw new ArgumentNullException(nameof(continuation));

            task.ContinueWith(t =>
            {
                MainThreadDispatcher.Enqueue(() => continuation(t));
            });
        }
    }
}