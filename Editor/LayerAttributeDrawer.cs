using UnityEditor;
using UnityEngine;

namespace Core.Utils.Editor
{
    /// <summary>
    /// Custom property drawer for <see cref="LayerAttribute"/>.
    /// </summary>
    /// <remarks>
    /// This drawer replaces the default integer field with Unity’s built-in
    /// <see cref="EditorGUI.LayerField"/>, which displays a dropdown listing all
    /// project layers.  
    ///
    /// Apply <c>[Layer]</c> to any <c>int</c> or <c>LayerMask</c>-compatible field
    /// to ensure designers select a layer through a safe, readable UI instead of
    /// manually entering layer indices.
    ///
    /// Example:
    /// <code>
    /// [Layer]
    /// public int enemyLayer;
    /// </code>
    /// </remarks>
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the layer selection field in the Inspector.
        /// </summary>
        /// <param name="position">The position within the Inspector UI.</param>
        /// <param name="property">The serialized property representing the field.</param>
        /// <param name="label">The display label for the property.</param>
        /// <remarks>
        /// The drawer safely updates <see cref="SerializedProperty.intValue"/> with the
        /// layer index selected via Unity’s layer dropdown.
        ///
        /// Only valid for properties whose type is <c>int</c>. Unity will display a
        /// warning if the attribute is applied to an unsupported field type.
        /// </remarks>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}