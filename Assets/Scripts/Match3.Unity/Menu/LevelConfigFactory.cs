using System.Collections.Generic;
using Core.Board;
using Core.Matching;
using Match3.Unity.GameMode;
using Match3.Unity.Goals;

namespace Match3.Unity.Menu
{
    public static class LevelConfigFactory
    {
        public static LevelConfig Create(LevelId levelId)
        {
            switch (levelId)
            {
                case LevelId.Level1:
                    return new LevelConfig(levelId, 
                        "Level 1", 
                        GameModeType.Moves, 
                        5, 
                        0f,
                        new List<GoalDefinition>
                        {
                            new GoalDefinition(GoalType.Score, 3),
                            new GoalDefinition(GoalType.Color, 5, PieceColor.Red),
                            new GoalDefinition(GoalType.Color, 6, PieceColor.Blue),
                            new GoalDefinition(GoalType.Color, 7, PieceColor.Green),
                            new GoalDefinition(GoalType.Obstacle, 2, obstacleKind: ObstacleKind.Box)
                        },
                        new List<ObstacleDefinition>
                        {
                            new ObstacleDefinition(new Pos(2, 2), ObstacleKind.Box, 1),
                            new ObstacleDefinition(new Pos(3, 4), ObstacleKind.Box, 1)
                        });

                case LevelId.Level2:
                    return new LevelConfig(levelId, 
                        "Level 2", 
                        GameModeType.Timer, 
                        0, 
                        10f,
                        new List<GoalDefinition>
                        {
                            new GoalDefinition(GoalType.Score, 3),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Red),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Blue),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Green),
                            new GoalDefinition(GoalType.Obstacle, 2, obstacleKind: ObstacleKind.Box)
                        },
                        new List<ObstacleDefinition>
                        {
                            new ObstacleDefinition(new Pos(2, 3), ObstacleKind.Box, 1),
                            new ObstacleDefinition(new Pos(3, 5), ObstacleKind.Box, 1),
                            new ObstacleDefinition(new Pos(4, 4), ObstacleKind.Box, 1)
                        });

                case LevelId.Level3:
                    return new LevelConfig(levelId, 
                        "Level 3", 
                        GameModeType.Moves, 
                        5, 
                        0f, 
                        new List<GoalDefinition>
                        {
                            new GoalDefinition(GoalType.Score, 50),
                            new GoalDefinition(GoalType.Color, 12, PieceColor.Red),
                            new GoalDefinition(GoalType.Color, 16, PieceColor.Blue),
                            new GoalDefinition(GoalType.Color, 20, PieceColor.Green),
                            new GoalDefinition(GoalType.Obstacle, 3, obstacleKind: ObstacleKind.Box)
                        },
                        new List<ObstacleDefinition>
                        {
                            new ObstacleDefinition(new Pos(2, 2), ObstacleKind.Box, 1),
                            new ObstacleDefinition(new Pos(2, 3), ObstacleKind.Box, 1),
                            new ObstacleDefinition(new Pos(3, 2), ObstacleKind.Box, 1)
                        });

                default:
                    throw new System.Exception("Unknown level");
            }
        }
    }
}