using Core.Board;

namespace Match3.Unity.Goals
{
    public interface IGoal
    {
        GoalType Type { get; }
        void Apply(ClearResolutionStats stats);
        bool IsCompleted { get; }
    }
}