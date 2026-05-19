using Core.Board;
using Core.Matching;

namespace Match3.Unity.Goals
{
    public class ColorGoalProgress : IGoal
    {
        public PieceColor Color { get; }
        public int Current { get; private set; }
        public int Target { get; }

        public ColorGoalProgress(PieceColor color, int target)
        {
            Color = color;
            Target = target;
            Current = 0;
        }

        public bool IsCompleted =>  Current >= Target;
        public GoalType Type => GoalType.Color;

        public void Apply(ClearResolutionStats stats)
        {
            Current += stats.GetClearedCount(Color);
        }
    }
}