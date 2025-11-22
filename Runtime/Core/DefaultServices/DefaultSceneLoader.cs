using System;
using System.Threading.Tasks;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Core.Utils.Logs.Logger;

namespace Core.DefaultServices
{
    /// <summary>
    /// Default implementation of <see cref="ISceneLoader"/> using Unity's <see cref="SceneManager"/>.
    /// </summary>
    /// <remarks>
    /// This implementation loads scenes asynchronously by name and optionally provides
    /// hooks for showing/hiding a loading screen. It can be extended later to support
    /// fades, additive loading, or more advanced transitions.
    /// </remarks>
    public class DefaultSceneLoader : ISceneLoader
    {
        /// <summary>
        /// Loads a scene asynchronously by name using <see cref="SceneManager.LoadSceneAsync(string)"/>.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the scene to load. Must match a scene included in the build settings.
        /// </param>
        /// <param name="showLoadingScreen">
        /// Whether a loading screen should be shown while the scene loads. This default
        /// implementation only provides a hook where such a feature can be plugged in.
        /// </param>
        /// <returns>
        /// A task that completes once the scene has finished loading and is activated.
        /// </returns>
        public async Task LoadSceneAsync(string sceneName, bool showLoadingScreen = true)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Logger.LogError("[StubSceneLoader] LoadSceneAsync called with null or empty sceneName.");
                return;
            }

            try
            {
                // TODO: Plug in your loading screen service here if you add one.
                // Example (if you later add an ILoadingScreenService):
                // if (showLoadingScreen)
                // {
                //     CoreServices.Get<ILoadingScreenService>()?.Show();
                // }

                AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
                if (asyncOp == null)
                {
                    Logger.LogError($"[StubSceneLoader] Failed to start loading scene '{sceneName}'.");
                    return;
                }

                // By default we allow activation as soon as loading is done.
                asyncOp.allowSceneActivation = true;

                var tcs = new TaskCompletionSource<bool>();
                asyncOp.completed += _ => tcs.TrySetResult(true);

                await tcs.Task;

                // TODO: Hide loading screen here if you implement one.
                // if (showLoadingScreen)
                // {
                //     CoreServices.Get<ILoadingScreenService>()?.Hide();
                // }

#if UNITY_EDITOR
                Logger.Log($"[StubSceneLoader] Finished loading scene '{sceneName}'.");
#endif
            }
            catch (Exception ex)
            {
                Logger.LogError($"[StubSceneLoader] Exception while loading scene '{sceneName}': {ex}");
            }
        }
    }
}
