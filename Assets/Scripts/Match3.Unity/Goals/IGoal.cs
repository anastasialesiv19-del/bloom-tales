namespace Match3.Unity.Goals
{
    public interface IGoal
    {
        GoalType Type { get; }
        void Apply(int clearedCells);
        bool IsCompleted { get; }
    }
}