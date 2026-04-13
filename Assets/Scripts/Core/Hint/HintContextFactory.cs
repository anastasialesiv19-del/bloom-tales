using System;
using Core.Hint;
using Core.Progress;

namespace Core.Hint
{
    public static class HintContextFactory
    {
        public static HintContext Create(float idleSeconds, AdaptiveHintState state)
        {
            if(state == null) throw new ArgumentNullException(nameof(state), "Adaptive Hints cannot be null.");
            return new HintContext(idleSeconds, state.ConsecutiveHintsShown);
        }
    }
}