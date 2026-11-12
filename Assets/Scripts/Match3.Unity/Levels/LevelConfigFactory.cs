using System.Collections.Generic;
using Core.Board;
using Match3.Unity.GameMode;
using Match3.Unity.Goals;
using Match3.Unity.Menu;

namespace Match3.Unity.Levels
{
    public static class LevelConfigFactory
    {
        public static LevelConfig Create(LevelId levelId)
        {
            switch (levelId)
            {
                case LevelId.Level1:
                    return new LevelConfig(levelId, "Morning Dew", GameModeType.Moves, 20, 0f,
                        new List<GoalDefinition>
                        {
                            new (GoalType.Score, 70),
                            new (GoalType.Color, 12, PieceColor.Red),
                            new (GoalType.Color, 12, PieceColor.Cyan),
                            new (GoalType.Obstacle, 4, obstacleKind: ObstacleKind.Box)
                        },
                        new List<ObstacleDefinition>
                        {
                            new (new Pos(2, 2), ObstacleKind.Box, 1),
                            new (new Pos(2, 3), ObstacleKind.Box, 1),
                            new (new Pos(5, 4), ObstacleKind.Box, 1),
                            new (new Pos(5, 5), ObstacleKind.Box, 1)
                        });

                case LevelId.Level2:
                    return new LevelConfig(levelId, "Golden Garden", GameModeType.Timer, 0, 150f,
                        new List<GoalDefinition>
                        {
                            new (GoalType.Score, 100),
                            new (GoalType.Color, 15, PieceColor.Yellow),
                            new (GoalType.Color, 15, PieceColor.Green),
                            new (GoalType.Obstacle, 6, obstacleKind: ObstacleKind.Box)
                        },
                        new List<ObstacleDefinition>
                        {
                            new (new Pos(2, 2), ObstacleKind.Box, 1),
                            new (new Pos(3, 2), ObstacleKind.Box, 1),
                            new (new Pos(4, 2), ObstacleKind.Box, 1),
                            new (new Pos(2, 5), ObstacleKind.Box, 1),
                            new (new Pos(3, 5), ObstacleKind.Box, 1),
                            new (new Pos(4, 5), ObstacleKind.Box, 1)
                        });

                case LevelId.Level3:
                    return new LevelConfig(levelId, "Moonlit Magic", GameModeType.Moves, 24, 0f, 
                        new List<GoalDefinition>
                        {
                            new (GoalType.Score, 130),
                            new (GoalType.Color, 18, PieceColor.Blue),
                            new (GoalType.Color, 18, PieceColor.Purple),
                            new (GoalType.Obstacle, 8, obstacleKind: ObstacleKind.Box)
                        },
                        new List<ObstacleDefinition>
                        {
                            new (new Pos(2, 2), ObstacleKind.Box, 1),
                            new (new Pos(2, 3), ObstacleKind.Box, 1),
                            new (new Pos(3, 2), ObstacleKind.Box, 1),
                            new (new Pos(3, 3), ObstacleKind.Box, 1),
                            new (new Pos(4, 4), ObstacleKind.Box, 1),
                            new (new Pos(4, 5), ObstacleKind.Box, 1),
                            new (new Pos(5, 4), ObstacleKind.Box, 1),
                            new (new Pos(5, 5), ObstacleKind.Box, 1)
                        });
                default:
                    throw new System.Exception("Unknown level");
            }
        }
    }
}