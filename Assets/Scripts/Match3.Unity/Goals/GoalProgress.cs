using Core.Board;

namespace Match3.Unity.Goals
{
    public class GoalProgress : IGoal
    {
        public int Current { get; private set; }
        public int Target { get; }

        public GoalProgress(int target)
        {
            Target = target;
            Current = 0;
        }
        
        public bool IsCompleted => Current >= Target;
        
        public GoalType Type => GoalType.Score;

        public void Apply(ClearResolutionStats stats)
        {
            Add(stats.TotalClearedCells);
        }
        
        private void Add(int amount)
        {
            Current += amount;
        }
    }
}