using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Hint
{
    public sealed class HintMoveOutcome
    {
        private readonly Dictionary<PieceColor, int> clearedByColor;
        public IReadOnlyDictionary<PieceColor, int> ClearedByColor => clearedByColor;
        public int EstimatedObstacleDamage { get; }
        public int EstimatedClearedPieces { get; }

        public static HintMoveOutcome Empty =>
            new HintMoveOutcome(new Dictionary<PieceColor, int>(), 0, 0);

        public HintMoveOutcome(IReadOnlyDictionary<PieceColor, int> clearedByColor, int estimatedObstacleDamage, int  estimatedClearedPieces)
        {
            if (clearedByColor == null)
                throw new ArgumentNullException(nameof(clearedByColor));

            if (estimatedObstacleDamage < 0)
                throw new ArgumentOutOfRangeException(nameof(estimatedObstacleDamage));
            
            if (estimatedClearedPieces < 0)
                throw new ArgumentOutOfRangeException(nameof(estimatedClearedPieces));

            this.clearedByColor = new Dictionary<PieceColor, int>(clearedByColor);
            EstimatedObstacleDamage = estimatedObstacleDamage;
            EstimatedClearedPieces = estimatedClearedPieces;
        }

        public int GetClearedCount(PieceColor color)
        {
            return clearedByColor.TryGetValue(color, out var count) ? count : 0;
        }
    }
}