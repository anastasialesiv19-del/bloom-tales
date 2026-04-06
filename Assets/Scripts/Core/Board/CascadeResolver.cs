using System;
using System.Collections.Generic;
using Core.Board;
using Core.Contracts;
using Core.Matching;

namespace Core.Matching
{
    public static class CascadeResolver
    {
        public static CascadeResult Resolve(BoardModel board)
        {
            return Resolve(board, null);
        }

        public static CascadeResult Resolve(BoardModel board, Action<int, int, BoardSnapshot> onWaveResolved)
        {
            int cascadeCount = 0;
            int totalClearedCells = 0;
            bool hadAnyMatches = false;
            var clearedPerWave = new List<int>();

            int maxIterations = 100;
            while (maxIterations > 0)
            {
                var matches = MatchFinder.FindMatches(board);
                if (matches.Count == 0)
                    break;

                hadAnyMatches = true;
                var cleared = BoardResolver.ClearMatches(board, matches);
                BoardResolver.ApplyGravity(board);
                BoardResolver.Refill(board);

                cascadeCount++;
                totalClearedCells += cleared;
                clearedPerWave.Add(cleared);
                var waveSnapshot = BoardSnapshotFactory.Capture(board);
                onWaveResolved?.Invoke(cascadeCount, cleared, waveSnapshot);
                maxIterations--;
            }
            return new CascadeResult(cascadeCount, totalClearedCells, hadAnyMatches, clearedPerWave);
        }
    }
}