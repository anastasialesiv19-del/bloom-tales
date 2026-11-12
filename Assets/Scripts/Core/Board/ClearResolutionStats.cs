using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Board
{
    public sealed class ClearResolutionStats
    {
        public int TotalClearedCells { get; }
        public int DestroyedObstacles { get; }
        
        private readonly Dictionary<PieceColor, int> _clearedByColor;
        private IReadOnlyDictionary<PieceColor, int> ClearedByColor => _clearedByColor;
        
        public static ClearResolutionStats Empty => new(0, new Dictionary<PieceColor, int>(), 0);

        public ClearResolutionStats(int totalClearedCells, IReadOnlyDictionary<PieceColor, int> clearedByColor, int destroyedObstacles)
        {
            if (totalClearedCells < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalClearedCells));
            }
            if (destroyedObstacles < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(destroyedObstacles));
            }
            if (clearedByColor == null)
            {
                throw new ArgumentNullException(nameof(clearedByColor));
            }

            TotalClearedCells = totalClearedCells;
            DestroyedObstacles = destroyedObstacles;
            _clearedByColor = new Dictionary<PieceColor, int>(clearedByColor);
        }

        public int GetClearedCount(PieceColor color)
        {
            return _clearedByColor.GetValueOrDefault(color, 0);
        }

        public ClearResolutionStats Add(ClearResolutionStats other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var merged = new Dictionary<PieceColor, int>(_clearedByColor);
            foreach (var pair in other.ClearedByColor)
            {
                if (merged.ContainsKey(pair.Key))
                {
                    merged[pair.Key] += pair.Value;
                }
                else
                {
                    merged[pair.Key] = pair.Value;
                }
            }
            return new ClearResolutionStats(TotalClearedCells + other.TotalClearedCells, merged, DestroyedObstacles + other.DestroyedObstacles);
        }
    }
}