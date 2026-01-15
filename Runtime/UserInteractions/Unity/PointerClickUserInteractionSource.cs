using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityCoreKit.Runtime.UserInteractions.Unity
{
    /// <summary>
    /// Emits a <see cref="UserInteractionEvent"/> when this object is clicked/tapped via Unity's EventSystem.
    /// Requires an EventSystem in scene and appropriate Raycasters (GraphicRaycaster / PhysicsRaycaster).
    /// </summary>
    public class PointerClickUserInteractionSource : MonoBehaviour, IPointerClickHandler
    {
        private IUserInteractions interactions;
        private IUserInteractionTarget target;
        
        private void OnDisable()
        {
            interactions = null;
            target = null;
        }
        
        /// <summary>
        /// Initializes this source with the interactions service and the target identity.
        /// </summary>
        public void Init(IUserInteractions interactions, IUserInteractionTarget target)
        {
            this.interactions = interactions;
            this.target = target;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (interactions == null || target == null)
                return;

            // This can be Vector3.zero for some UI cases; still provides useful info.
            var worldPos = eventData.pointerCurrentRaycast.worldPosition;

            var evt = new UserInteractionEvent(UserInteractionType.Click, target, worldPos);
            interactions.Publish(in evt);
            
            Debug.Log($"[PointerClickUserInteractionSource] Publishing click for {target.InteractionKey}: {target}");
        }
        
    }
}