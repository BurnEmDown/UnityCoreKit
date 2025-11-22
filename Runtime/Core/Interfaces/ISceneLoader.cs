using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a contract for asynchronously loading Unity scenes.
    /// </summary>
    /// <remarks>
    /// Implementations may optionally display loading screens, perform fade effects,
    /// handle scene activation delays, or manage additive/replace loading strategies.
    /// </remarks>
    public interface ISceneLoader
    {
        /// <summary>
        /// Loads a Unity scene asynchronously by name.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the scene to load. Must match a scene included in the build settings.
        /// </param>
        /// <param name="showLoadingScreen">
        /// Whether a loading screen or transition effect should be displayed while loading.
        /// Implementations may ignore this flag or provide custom behavior.
        /// </param>
        /// <returns>
        /// A task that completes once the scene has finished loading and is activated.
        /// </returns>
        Task LoadSceneAsync(string sceneName, bool showLoadingScreen = true);
    }
}