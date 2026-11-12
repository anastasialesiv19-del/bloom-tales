using Core.Board;

namespace Match3.Unity.Goals
{
    public sealed class ObstacleGoalProgress : IGoal
    {
        public ObstacleKind Kind { get; }
        public int Current { get; private set; }
        public int Target { get; }

        public bool IsCompleted => Current >= Target;
        public GoalType Type => GoalType.Obstacle;

        public ObstacleGoalProgress(ObstacleKind kind, int target)
        {
            Kind = kind;
            Target = target;
            Current = 0;
        }

        public void Apply(ClearResolutionStats stats)
        {
            Current += stats.DestroyedObstacles;
        }
    }
}