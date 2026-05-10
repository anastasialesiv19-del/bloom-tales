using Core.Matching;
using Match3.Unity.Goals;

namespace Match3.Unity.Menu
{
    public sealed class GoalDefinition
    {
        public GoalType Type { get; }
        public PieceColor? Color { get; }
        public int Target { get; }

        public GoalDefinition(GoalType type, int target, PieceColor? color = null)
        {
            Type = type;
            Target = target;
            Color = color;
        }
    }
}