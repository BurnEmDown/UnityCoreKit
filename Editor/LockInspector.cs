using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.Utils.Editor
{
    /// <summary>
    /// Provides a menu command to toggle the lock state of the active Inspector,
    /// and optionally toggles the "constrain proportions" scale option for
    /// <see cref="Transform"/> components in that Inspector.
    /// </summary>
    /// <remarks>
    /// The menu item is added under <c>Edit/Lock Inspector</c> and is bound to the
    /// shortcut <c>Ctrl+L</c> (Windows) / <c>Cmd+L</c> (macOS) via the <c>%l</c> symbol
    /// in the <see cref="MenuItemAttribute"/>.
    ///
    /// <para>
    /// This utility uses <see cref="ActiveEditorTracker"/> to toggle the Inspector’s
    /// <see cref="ActiveEditorTracker.isLocked"/> flag. It then iterates over the
    /// active editors and, for any <see cref="Transform"/> targets, uses reflection
    /// to toggle the non-public <c>constrainProportionsScale</c> property. This can
    /// be handy when working with locked Inspectors and wanting consistent scale
    /// behavior.
    /// </para>
    ///
    /// <para>
    /// Note: since this relies on a non-public property via reflection, it may break
    /// if Unity changes the internal API or property name in future versions.
    /// </para>
    /// </remarks>
    public class LockInspector
    {
        /// <summary>
        /// Toggles the lock state of the global <see cref="ActiveEditorTracker"/>
        /// and flips the "constrain proportions" flag on any <see cref="Transform"/>
        /// found in the active editors list.
        /// </summary>
        [MenuItem("Edit/Lock Inspector %l")]
        public static void Lock()
        {
            ActiveEditorTracker tracker = ActiveEditorTracker.sharedTracker;
            tracker.isLocked = !tracker.isLocked;

            // Lock/unlock scale constrain for Transform in the locked inspector
            foreach (var activeEditor in tracker.activeEditors)
            {
                if (activeEditor.target is not Transform) continue;

                var transform = (Transform)activeEditor.target;

                var propInfo = transform.GetType().GetProperty(
                    "constrainProportionsScale",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (propInfo == null) continue;

                var value = (bool)propInfo.GetValue(transform, null);
                propInfo.SetValue(transform, !value, null);
            }

            tracker.ForceRebuild();
        }

        /// <summary>
        /// Validates whether the "Lock Inspector" menu item should be enabled.
        /// </summary>
        /// <remarks>
        /// The menu item is only enabled when there is at least one active editor
        /// in the shared tracker.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if there is at least one active editor; otherwise <c>false</c>.
        /// </returns>
        [MenuItem("Edit/Lock Inspector %l", true)]
        public static bool Valid()
        {
            return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
        }
    }
}