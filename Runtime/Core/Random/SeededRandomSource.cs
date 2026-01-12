using System;

namespace UnityCoreKit.Runtime.Core.Random
{
    /// <summary>
    /// Deterministic random source backed by <see cref="System.Random"/>.
    /// </summary>
    /// <remarks>
    /// Using a fixed seed guarantees reproducible sequences of random values.
    /// Ideal for puzzles, replays, tests, and board-game style logic.
    /// </remarks>
    public sealed class SeededRandomSource : IRandomSource
    {
        private readonly System.Random random;

        /// <summary>
        /// Creates a new seeded random source.
        /// </summary>
        /// <param name="seed">
        /// Seed value controlling the random sequence.
        /// Using the same seed produces the same results.
        /// </param>
        public SeededRandomSource(int seed)
        {
            random = new System.Random(seed);
        }

        /// <inheritdoc />
        public int NextInt(int minInclusive, int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }

        /// <inheritdoc />
        public float NextFloat()
        {
            return (float)random.NextDouble();
        }

        /// <inheritdoc />
        public bool Chance(float probability)
        {
            if (probability <= 0f) return false;
            if (probability >= 1f) return true;

            return NextFloat() < probability;
        }
    }
}