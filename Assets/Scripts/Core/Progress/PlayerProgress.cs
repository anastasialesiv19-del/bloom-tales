using System;

namespace Core.Progress
{
    public sealed class PlayerProgress
    {
        public AdaptiveHintState AdaptiveHints { get; }
        public int TurnsPlayed { get; }

        public PlayerProgress(AdaptiveHintState adaptiveHints, int turnsPlayed)
        {
            AdaptiveHints = adaptiveHints ?? throw new ArgumentNullException(nameof(adaptiveHints), "Adaptive Hints cannot be null.");
            TurnsPlayed = turnsPlayed >= 0 ? turnsPlayed : throw new ArgumentOutOfRangeException(nameof(turnsPlayed), "Invalid Turns Played hint shown.");
        }
    }
}