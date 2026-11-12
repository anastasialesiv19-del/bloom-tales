using System;
using Core.Progress;

namespace Core.Hint
{
    public static class HintContextFactory
    {
        public static HintContext Create(float idleSeconds, AdaptiveHintState state)
        {
            return state == null ? throw new ArgumentNullException(nameof(state), "Adaptive Hints cannot be null.") : new HintContext(idleSeconds, state.ConsecutiveHintsShown);
        }
    }
}