namespace UnityCoreKit.Runtime.Core.Random
{
    /// <summary>
    /// Abstraction for a random number generator.
    /// Allows deterministic, testable randomness across systems.
    /// </summary>
    /// <remarks>
    /// This interface exists to decouple game logic from concrete RNG implementations
    /// (e.g. UnityEngine.Random, System.Random).
    /// </remarks>
    public interface IRandomSource
    {
        /// <summary>
        /// Returns a random integer in the range [minInclusive, maxExclusive).
        /// </summary>
        int NextInt(int minInclusive, int maxExclusive);

        /// <summary>
        /// Returns a random floating-point value in the range [0.0, 1.0).
        /// </summary>
        float NextFloat();

        /// <summary>
        /// Returns true with the given probability (0..1).
        /// </summary>
        bool Chance(float probability);
    }
}