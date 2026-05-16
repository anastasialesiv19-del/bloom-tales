namespace Match3.Unity.Menu
{
    public class SelectedLevelState
    {
        public static LevelConfig Current { get; private set; }

        public static void Set(LevelConfig config)
        {
            Current = config;
        }

        public static void Clear()
        {
            Current = null;
        }
    }
}