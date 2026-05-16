using System.Collections.Generic;
using Match3.Unity.GameMode;

namespace Match3.Unity.Menu
{
    public sealed class LevelConfig
    {
        public LevelId LevelId { get; }
        public string LevelName { get;}
        public GameModeType GameMode { get;}
        public int TotalMoves { get;}
        public float TotalGameSeconds { get; }
        public int TargetScore { get; }
        public IReadOnlyList<GoalDefinition> Goals { get; }

        public LevelConfig(LevelId levelId, string levelName, GameModeType gameMode, int totalMoves, float totalGameSeconds, int  targetScore,  IReadOnlyList<GoalDefinition> goals)
        {
            LevelId = levelId;
            LevelName = levelName;
            GameMode = gameMode;
            TotalMoves = totalMoves;
            TotalGameSeconds = totalGameSeconds;
            TargetScore = targetScore;
            Goals = goals;
        }
    }
}