using System;
using System.Collections.Generic;
using Core.Board;
using Core.Matching;

namespace Core.Hint
{
    public sealed class HintGoalContext
    {
        public int RemainingObstacles { get; }
        public int RemainingScore { get; }
        public IReadOnlyDictionary<PieceColor, int> RemainingByColor => _remainingByColor;
        private readonly Dictionary<PieceColor, int> _remainingByColor;

        public static HintGoalContext Empty => new(new Dictionary<PieceColor, int>(), 0, 0);

        public HintGoalContext(IReadOnlyDictionary<PieceColor, int> remainingByColor, int remainingObstacles, int remainingScore = 0)
        {
            if (remainingByColor == null)
            {
                throw new ArgumentNullException(nameof(remainingByColor));
            }
            if (remainingObstacles < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(remainingObstacles));
            }
            if (remainingScore < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(remainingScore));
            }
            
            _remainingByColor = new Dictionary<PieceColor, int>();
            foreach (var pair in remainingByColor)
            {
                if (pair.Value > 0)
                {
                    _remainingByColor[pair.Key] = pair.Value;
                }
            }
            RemainingObstacles = remainingObstacles;
            RemainingScore = remainingScore;
        }

        public int GetRemainingColor(PieceColor color)
        {
            return _remainingByColor.TryGetValue(color, out var remaining) ? remaining : 0;
        }
        
        public bool HasColorGoals => _remainingByColor.Count > 0;
        
        public bool HasObstacleGoal => RemainingObstacles > 0;
    }
}