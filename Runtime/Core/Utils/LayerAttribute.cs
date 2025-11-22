using UnityEngine;

namespace Core.Utils
{
    /// <summary>
    /// Attribute used to mark an integer field as a Unity Layer selector.
    /// </summary>
    /// <remarks>
    /// When applied to an <c>int</c> field, the associated custom property drawer
    /// (<see cref="Core.Utils.Editor.LayerAttributeDrawer"/>) will display Unity’s
    /// layer dropdown instead of a raw numeric input.
    ///
    /// This ensures:
    /// <list type="bullet">
    ///     <item>Layer indices are chosen safely and visually</item>
    ///     <item>Designers cannot accidentally type an invalid layer number</item>
    ///     <item>Your code is more readable and clear in the Inspector</item>
    /// </list>
    ///
    /// Example usage:
    /// <code>
    /// [Layer]
    /// public int enemyLayer;
    /// </code>
    ///
    /// This attribute does not contain any logic by itself; its behavior is completely
    /// handled by the <c>LayerAttributeDrawer</c> in the Editor assembly.
    /// </remarks>
    public class LayerAttribute : PropertyAttribute { }
}