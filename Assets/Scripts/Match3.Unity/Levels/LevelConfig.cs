using System;
using System.Collections.Generic;
using Core.Board;
using Match3.Unity.GameMode;
using Match3.Unity.Goals;
using Match3.Unity.Menu;

namespace Match3.Unity.Levels
{
    public sealed class LevelConfig
    {
        public LevelId LevelId { get; }
        public string LevelName { get;}
        public GameModeType GameMode { get;}
        public int TotalMoves { get;}
        public float TotalGameSeconds { get; }
        public IReadOnlyList<GoalDefinition> Goals { get; }
        public IReadOnlyList<ObstacleDefinition> Obstacles { get; }

        public LevelConfig(LevelId levelId, string levelName, GameModeType gameMode, int totalMoves, float totalGameSeconds, IReadOnlyList<GoalDefinition> goals, IReadOnlyList<ObstacleDefinition> obstacles)
        {
            LevelId = levelId;
            LevelName = levelName;
            GameMode = gameMode;
            TotalMoves = totalMoves;
            TotalGameSeconds = totalGameSeconds;
            Goals = goals;
            Obstacles = obstacles ?? Array.Empty<ObstacleDefinition>();
        }
    }
}