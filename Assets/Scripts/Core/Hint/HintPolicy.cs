namespace Core.Hint
{
    public static class HintPolicy
    {
        public static HintStrength GetStrength(HintContext context, int bestMoveScore)
        {
            HintStrength baseStrength;
            if (context.IdleSeconds < 3)
            {
                baseStrength = HintStrength.None;
            }
            else if (context.IdleSeconds < 7)
            {
                baseStrength = HintStrength.Soft;
            }
            else
            {
                baseStrength = HintStrength.Strong;
            }

            if (baseStrength != HintStrength.None && context.ConsecutiveHintsShown >= 2)
            {
                baseStrength = HintStrength.Strong;
            }
            
            if (baseStrength == HintStrength.None)
            {
                return HintStrength.None;
            }
            
            if (baseStrength == HintStrength.Soft && bestMoveScore >= 5)
            {
                return HintStrength.Strong;
            }
            return baseStrength;
        }
    }
}