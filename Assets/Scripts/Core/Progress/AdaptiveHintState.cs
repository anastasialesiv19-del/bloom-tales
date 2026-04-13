using System;

namespace Core.Progress
{
    public sealed class AdaptiveHintState
    {
        public int ConsecutiveHintsShown { get; }
        public int TotalHintsShown { get; }

        public AdaptiveHintState(int consecutiveHintsShown, int totalHintsShown)
        {
            ConsecutiveHintsShown = consecutiveHintsShown >= 0 ? consecutiveHintsShown : 
                throw new ArgumentOutOfRangeException(nameof(consecutiveHintsShown),"Invalid consecutive hints shown");
            TotalHintsShown = totalHintsShown >= 0 ? totalHintsShown : 
                throw new ArgumentOutOfRangeException(nameof(totalHintsShown), "Invalid total hints shown");
        }
    }
}