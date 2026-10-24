using System;
using System.Collections.Generic;
using Core.Board;
using Core.Contracts;
using Core.Matching;
using Core.Randomness;
using Core.Specials;

namespace Core.Matching
{
    public static class CascadeResolver
    {
        public static CascadeResult Resolve(BoardModel board)
        {
            return Resolve(board, null, null);
        }

        public static CascadeResult Resolve(BoardModel board, Action<int, int, BoardSnapshot> onWaveResolved)
        {
            return Resolve(board, null, onWaveResolved);
        }

        public static CascadeResult Resolve(BoardModel board, IRandomProvider provider,
            Action<int, int, BoardSnapshot> onWaveResolved = null, IReadOnlyList<PieceFall> initialFalls = null, IReadOnlyList<Pos> initialRefills = null)
        {
            int cascadeCount = 0;
            int totalClearedCells = 0;
            bool hadAnyMatches = false;
            var clearedPerWave = new List<int>();

            var totalStats = ClearResolutionStats.Empty;
            int maxIterations = 100;
            IReadOnlyList<PieceFall> fallsBeforeWave = initialFalls ?? Array.Empty<PieceFall>();
            IReadOnlyList<Pos> refillsBeforeWave = initialRefills ?? Array.Empty<Pos>();
            while (maxIterations > 0)
            {
                var matches = MatchFinder.FindMatches(board);
                if (matches.Count == 0)
                    break;

                hadAnyMatches = true;
                var spawns = SpecialSpawnResolver
                    .BuildDecisions(board, matches, fallsBeforeWave: fallsBeforeWave, refillsBeforeWave: refillsBeforeWave);
                var waveStats = BoardResolver.ClearMatchesAndSpawnSpecials(board, matches, spawns);
                var cleared = waveStats.TotalClearedCells;
                totalStats = totalStats.Add(waveStats);
                fallsBeforeWave = BoardResolver.ApplyGravity(board);
                refillsBeforeWave = BoardResolver.Refill(board,  provider);

                cascadeCount++;
                totalClearedCells += cleared;
                clearedPerWave.Add(cleared);
                var waveSnapshot = BoardSnapshotFactory.Capture(board);
                onWaveResolved?.Invoke(cascadeCount, cleared, waveSnapshot);
                maxIterations--;
            }
            return new CascadeResult(cascadeCount, totalClearedCells, hadAnyMatches, clearedPerWave, totalStats);
        }
    }
}