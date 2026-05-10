using System;
using Match3.Unity.Menu;

namespace Match3.Unity.Goals
{
    public static class GoalFactory
    {
        public static IGoal Create(GoalDefinition definition)
        {
            switch (definition.Type)
            {
                case GoalType.Score:
                    return new GoalProgress(definition.Target);

                case GoalType.Color:
                    return new ColorGoalProgress(definition.Color.Value, definition.Target);

                default:
                    throw new Exception("Unknown goal type");
            }
        }
    }
}