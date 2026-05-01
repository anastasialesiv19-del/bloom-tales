using Match3.Unity.GameMode;

namespace Match3.Unity.Menu
{
    public static class LevelConfigFactory
    {
        public static LevelConfig Create(LevelId levelId)
        {
            switch (levelId)
            {
                case LevelId.Level1:
                    return new LevelConfig(levelId, "Level 1", GameModeType.Moves, 40, 0f);

                case LevelId.Level2:
                    return new LevelConfig(levelId, "Level 2", GameModeType.Timer, 0, 10f);

                case LevelId.Level3:
                    return new LevelConfig(levelId, "Level 3", GameModeType.Moves, 5, 0f);

                default:
                    throw new System.Exception("Unknown level");
            }
        }
    }
}