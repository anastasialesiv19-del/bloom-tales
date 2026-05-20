using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Hint
{
    public sealed class HintGoalContext
    {
        private readonly Dictionary<PieceColor, int> remainingByColor;

        public IReadOnlyDictionary<PieceColor, int> RemainingByColor => remainingByColor;
        public int RemainingObstacles { get; }

        public static HintGoalContext Empty =>
            new HintGoalContext(new Dictionary<PieceColor, int>(), 0);

        public HintGoalContext(
            IReadOnlyDictionary<PieceColor, int> remainingByColor,
            int remainingObstacles)
        {
            if (remainingByColor == null)
                throw new ArgumentNullException(nameof(remainingByColor));

            if (remainingObstacles < 0)
                throw new ArgumentOutOfRangeException(nameof(remainingObstacles));

            this.remainingByColor = new Dictionary<PieceColor, int>();

            foreach (var pair in remainingByColor)
            {
                if (pair.Value > 0)
                    this.remainingByColor[pair.Key] = pair.Value;
            }
            RemainingObstacles = remainingObstacles;
        }

        public int GetRemainingColor(PieceColor color)
        {
            return remainingByColor.TryGetValue(color, out var remaining) ? remaining : 0;
        }
        public bool HasColorGoals => remainingByColor.Count > 0;
        public bool HasObstacleGoal => RemainingObstacles > 0;
    }
}