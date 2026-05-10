using System.Collections.Generic;
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
                        40, 
                        0f, 
                        30,
                        new List<GoalDefinition>
                        {
                            new GoalDefinition(GoalType.Score, 50),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Red),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Blue),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Green)
                        });

                case LevelId.Level2:
                    return new LevelConfig(levelId, 
                        "Level 2", 
                        GameModeType.Timer, 
                        0, 
                        10f, 
                        40,
                        new List<GoalDefinition>
                        {
                            new GoalDefinition(GoalType.Score, 50),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Red),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Blue),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Green)
                        });

                case LevelId.Level3:
                    return new LevelConfig(levelId, 
                        "Level 3", 
                        GameModeType.Moves, 
                        5, 
                        0f, 
                        50,
                        new List<GoalDefinition>
                        {
                            new GoalDefinition(GoalType.Score, 50),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Red),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Blue),
                            new GoalDefinition(GoalType.Color, 10, PieceColor.Green)
                        });

                default:
                    throw new System.Exception("Unknown level");
            }
        }
    }
}