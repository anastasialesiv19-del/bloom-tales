namespace Core.Hint
{
    public readonly struct HintContext
    {
        public float IdleSeconds { get;}
		public int ConsecutiveHintsShown {get;}

        public HintContext(float idleSeconds, int consecutiveHintsShown)
        {
            IdleSeconds = idleSeconds;
            ConsecutiveHintsShown = consecutiveHintsShown;
        }
    }
}