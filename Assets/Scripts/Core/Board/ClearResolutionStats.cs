using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Board
{
    public sealed class ClearResolutionStats
    {
        private readonly Dictionary<PieceColor, int> clearedByColor;
        public int TotalClearedCells { get; }
        public int DestroyedObstacles { get; }
        public IReadOnlyDictionary<PieceColor, int> ClearedByColor => clearedByColor;
        
        public static ClearResolutionStats Empty => new ClearResolutionStats(0, new Dictionary<PieceColor, int>(), 0);

        public ClearResolutionStats(int totalClearedCells, IReadOnlyDictionary<PieceColor, int> clearedByColor,
            int destroyedObstacles)
        {
            if (totalClearedCells < 0)
                throw new ArgumentOutOfRangeException(nameof(totalClearedCells));

            if (destroyedObstacles < 0)
                throw new ArgumentOutOfRangeException(nameof(destroyedObstacles));

            if (clearedByColor == null)
                throw new ArgumentNullException(nameof(clearedByColor));

            TotalClearedCells = totalClearedCells;
            DestroyedObstacles = destroyedObstacles;
            this.clearedByColor = new Dictionary<PieceColor, int>(clearedByColor);
        }

        public int GetClearedCount(PieceColor color)
        {
            return clearedByColor.GetValueOrDefault(color, 0);
        }

        public ClearResolutionStats Add(ClearResolutionStats other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var merged = new Dictionary<PieceColor, int>(clearedByColor);

            foreach (var pair in other.ClearedByColor)
            {
                if (merged.ContainsKey(pair.Key))
                    merged[pair.Key] += pair.Value;
                else
                    merged[pair.Key] = pair.Value;
            }

            return new ClearResolutionStats(
                TotalClearedCells + other.TotalClearedCells,
                merged,
                DestroyedObstacles + other.DestroyedObstacles);
        }
    }
}