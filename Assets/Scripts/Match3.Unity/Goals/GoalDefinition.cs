using Core.Board;

namespace Match3.Unity.Goals
{
    public sealed class GoalDefinition
    {
        public GoalType Type { get; }
        public PieceColor? Color { get; }
        public ObstacleKind? ObstacleKind { get; }
        public int Target { get; }

        public GoalDefinition(GoalType type, int target, PieceColor? color = null, ObstacleKind? obstacleKind = null)
        {
            Type = type;
            Target = target;
            Color = color;
            ObstacleKind =  obstacleKind;
        }
    }
}