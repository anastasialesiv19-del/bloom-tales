using System;
using System.Collections.Generic;
using Core.Board;
using Core.Matching;

namespace Core.Hint
{
    public sealed class HintMoveOutcome
    {
        public int EstimatedObstacleDamage { get; }
        public int EstimatedClearedPieces { get; }
        public IReadOnlyDictionary<PieceColor, int> ClearedByColor => _clearedByColor;
        private readonly Dictionary<PieceColor, int> _clearedByColor;

        public static HintMoveOutcome Empty => new (new Dictionary<PieceColor, int>(), 0, 0);

        public HintMoveOutcome(IReadOnlyDictionary<PieceColor, int> clearedByColor, int estimatedObstacleDamage, int  estimatedClearedPieces)
        {
            if (clearedByColor == null)
            {
                throw new ArgumentNullException(nameof(clearedByColor));
            }
            if (estimatedObstacleDamage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedObstacleDamage));
            }
            if (estimatedClearedPieces < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedClearedPieces));
            }

            _clearedByColor = new Dictionary<PieceColor, int>(clearedByColor);
            EstimatedObstacleDamage = estimatedObstacleDamage;
            EstimatedClearedPieces = estimatedClearedPieces;
        }

        public int GetClearedCount(PieceColor color)
        {
            return _clearedByColor.TryGetValue(color, out var count) ? count : 0;
        }
    }
}