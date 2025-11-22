using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Core.Utils.Editor
{
    /// <summary>
    /// Editor utility for quickly entering Play Mode through a designated
    /// initialization scene and returning to the previously edited scene.
    /// </summary>
    /// <remarks>
    /// Many Unity projects include a bootstrap or loader scene (e.g. an InitScene)
    /// that must run before any gameplay scenes.  
    /// 
    /// <para>
    /// This tool automates that workflow by:
    /// </para>
    /// <list type="bullet">
    ///     <item>Saving the name of the currently open scene</item>
    ///     <item>Opening the InitScene</item>
    ///     <item>Starting Play Mode automatically</item>
    ///     <item>Providing a menu command to restore the previously edited scene afterward</item>
    /// </list>
    ///
    /// A small hidden file <c>.lastScene</c> is written to the project root to
    /// remember the last open scene between runs.
    ///
    /// <para>
    /// Menu commands:
    /// </para>
    /// <list type="bullet">
    ///     <item><c>COD/PlayGame</c> — Save current scene → open InitScene → enter Play Mode.</item>
    ///     <item><c>COD/LoadEditedScene</c> — Reload the scene saved before the PlayGame command.</item>
    /// </list>
    ///
    /// This tool is intended to streamline testing workflows and reduce repetitive
    /// scene switching during development.
    /// </remarks>
    public class OpenLoaderTool
    {
        /// <summary>
        /// Saves the currently open scene name, loads the InitScene, and enters Play Mode.
        /// </summary>
        /// <remarks>
        /// Writes a hidden <c>.lastScene</c> file to the project root storing the name
        /// of the active scene (without extension).  
        ///
        /// The method then loads <c>Assets/Scenes/InitScene.unity</c> and immediately
        /// sets <see cref="EditorApplication.isPlaying"/> to <c>true</c>.
        /// </remarks>
        [MenuItem("COD/PlayGame")]
        public static void OpenLoader()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            File.WriteAllText(".lastScene", currentSceneName);
            EditorSceneManager.OpenScene($"{Directory.GetCurrentDirectory()}/Assets/Scenes/InitScene.unity");
            EditorApplication.isPlaying = true;
        }

        /// <summary>
        /// Re-opens the last edited scene prior to using <see cref="OpenLoader"/>.
        /// </summary>
        /// <remarks>
        /// Reads the <c>.lastScene</c> file and reopens the scene located at:
        /// <c>Assets/Scenes/&lt;SceneName&gt;.unity</c>.
        ///
        /// Useful after automatically entering Play Mode from the InitScene,
        /// allowing you to quickly return to where you were working.
        /// </remarks>
        [MenuItem("COD/LoadEditedScene")]
        public static void ReturnToLastScene()
        {
            string lastScene = File.ReadAllText(".lastScene");
            EditorSceneManager.OpenScene($"{Directory.GetCurrentDirectory()}/Assets/Scenes/{lastScene}.unity");
        }
    }
}