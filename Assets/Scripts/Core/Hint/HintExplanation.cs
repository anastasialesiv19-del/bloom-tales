using System.Collections.Generic;

namespace Core.Hint
{
    public sealed class HintExplanation
    {
        public IReadOnlyList<HintReasonType> Reasons { get; }
        public int Score { get; }

        public HintExplanation(IReadOnlyList<HintReasonType> reasons, int score)
        {
            Reasons = reasons;
            Score = score;
        }
    }
}