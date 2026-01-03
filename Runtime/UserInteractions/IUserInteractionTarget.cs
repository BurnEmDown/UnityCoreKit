namespace UnityCoreKit.Runtime.UserInteractions
{
    /// <summary>
    /// Minimal boundary that identifies something that can be interacted with by the user.
    /// </summary>
    public interface IUserInteractionTarget
    {
        /// <summary>
        /// A semantic key for routing/filtering (e.g. "Tile", "Card", "UIButton", "Cell").
        /// </summary>
        string InteractionKey { get; }

        /// <summary>
        /// Optional underlying model reference (often read-only). May be null.
        /// </summary>
        object Model { get; }
    }
}