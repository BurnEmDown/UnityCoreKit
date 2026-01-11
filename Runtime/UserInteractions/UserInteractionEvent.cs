using UnityEngine;

namespace UnityCoreKit.Runtime.UserInteractions
{
    /// <summary>
    /// Strongly-typed payload describing a user interaction.
    /// Published via <see cref="UnityCoreKit.Runtime.UserInteractions.Services.UserInteractionsService.Publish"/>.
    /// </summary>
    public readonly struct UserInteractionEvent
    {
        public readonly UserInteractionType Type;
        public readonly IUserInteractionTarget Target;
        public readonly Vector3 WorldPosition;

        public UserInteractionEvent(UserInteractionType type, IUserInteractionTarget target, Vector3 worldPosition)
        {
            Type = type;
            Target = target;
            WorldPosition = worldPosition;
        }
    }
}