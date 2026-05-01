namespace Match3.Unity.Menu
{
    public static class LevelProgression
    {
        public static bool TryGetNext(LevelId current, out LevelId next)
        {
            switch (current)
            {
                case LevelId.Level1:
                    next = LevelId.Level2;
                    return true;
                case LevelId.Level2:
                    next = LevelId.Level3;
                    return true;
                default:
                    next = current;
                    return false;
            }
        }
    }
}