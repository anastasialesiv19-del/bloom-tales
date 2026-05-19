using Core.Board;

namespace Match3.Unity.Goals
{
    public sealed class ObstacleGoalProgress : IGoal
    {
        public GoalType Type => GoalType.Obstacle;

        public ObstacleKind Kind { get; }
        public int Current { get; private set; }
        public int Target { get; }

        public bool IsCompleted => Current >= Target;

        public ObstacleGoalProgress(ObstacleKind kind, int target)
        {
            Kind = kind;
            Target = target;
            Current = 0;
        }

        public void Apply(ClearResolutionStats stats)
        {
            // MVP: зараз core рахує destroyed obstacles загалом.
            // Коли будуть різні obstacle kinds, розширимо ClearStats до breakdown by kind.
            Current += stats.DestroyedObstacles;
        }
    }
}