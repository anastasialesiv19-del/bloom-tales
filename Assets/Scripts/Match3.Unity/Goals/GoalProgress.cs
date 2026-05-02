namespace Match3.Unity.Goals
{
    public class GoalProgress
    {
        public int Current { get; private set; }
        public int Target { get; }

        public GoalProgress(int target)
        {
            Target = target;
            Current = 0;
        }

        public void Add(int amount)
        {
            Current += amount;
        }

        public bool IsCompleted => Current >= Target;
    }
}