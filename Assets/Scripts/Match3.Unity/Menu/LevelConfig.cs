using Core.Matching;
using Match3.Unity.GameMode;

namespace Match3.Unity.Menu
{
    public sealed class LevelConfig
    {
        public string LevelName { get; set; }
        public GameModeType GameMode { get; set; }
        public int TotalMoves { get; set; }
        public float TotalGameSeconds { get; set; }

        public LevelConfig(string levelName, GameModeType gameMode, int totalMoves, float totalGameSeconds)
        {
            LevelName = levelName;
            GameMode = gameMode;
            TotalMoves = totalMoves;
            TotalGameSeconds = totalGameSeconds;
        }
    }
}