using System;

namespace Core.Hint
{
    public static class HintPolicy
    {
        public static HintStrength GetStrength(HintContext context)
        {
            if (context.ConsecutiveHintsShown >= 2)
                return HintStrength.Strong;
            if (context.IdleSeconds < 3) 
                return HintStrength.None;
            if (context.IdleSeconds is >= 3 and < 7) 
                return HintStrength.Soft;
            if (context.IdleSeconds >= 7)
                return HintStrength.Strong;
            throw new InvalidOperationException("Invalid hint strength");
        }
    }
}