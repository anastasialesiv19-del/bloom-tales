using System;
using System.Collections.Generic;
using Core.Contracts;
using Core.Matching;
using Core.Randomness;
using Core.Specials;

namespace Core.Board
{
    public static class CascadeResolver
    {
        public static CascadeResult Resolve(BoardModel board)
        {
            return Resolve(board, null, null);
        }

        public static CascadeResult Resolve(BoardModel board, Action<int, int, BoardSnapshot, IReadOnlyList<SpecialSpawnDecision>, IReadOnlyList<SpecialActivationEvent>, CascadeWaveAnimation> onWaveResolved)
        {
            return Resolve(board, null, onWaveResolved);
        }

        public static CascadeResult Resolve(BoardModel board, IRandomProvider provider,
            Action<int, int, BoardSnapshot, IReadOnlyList<SpecialSpawnDecision>, IReadOnlyList<SpecialActivationEvent>, CascadeWaveAnimation> onWaveResolved = null, IReadOnlyList<PieceFall> initialFalls = null, IReadOnlyList<Pos> initialRefills = null)
        {
            var cascadeCount = 0;
            var totalClearedCells = 0;
            var hadAnyMatches = false;
            var clearedPerWave = new List<int>();
            var totalStats = ClearResolutionStats.Empty;
             
            var maxIterations = 100;
            var fallsBeforeWave = initialFalls ?? Array.Empty<PieceFall>();
            var refillsBeforeWave = initialRefills ?? Array.Empty<Pos>();
            
            while (maxIterations > 0)
            {
                var matches = MatchFinder.FindMatches(board);
                if (matches.Count == 0)
                {
                    break;
                }
                hadAnyMatches = true;
                
                var spawns = SpecialSpawnResolver.BuildDecisions(board, matches, fallsBeforeWave: fallsBeforeWave, refillsBeforeWave: refillsBeforeWave);
                var waveStats = BoardResolver.ClearMatchesAndSpawnSpecials(board, matches, spawns, out var waveActivations);
                var cleared = waveStats.TotalClearedCells;
                totalStats = totalStats.Add(waveStats);

                var afterClear = onWaveResolved != null ? BoardSnapshotFactory.Capture(board) : null;

                fallsBeforeWave = BoardResolver.ApplyGravity(board);

                var afterGravity = onWaveResolved != null ? BoardSnapshotFactory.Capture(board) : null;

                refillsBeforeWave = BoardResolver.Refill(board, provider);

                cascadeCount++;
                totalClearedCells += cleared;
                clearedPerWave.Add(cleared);

                var waveSnapshot = BoardSnapshotFactory.Capture(board);

                if (onWaveResolved != null)
                {
                    var animation = new CascadeWaveAnimation(afterClear, afterGravity, fallsBeforeWave);

                    onWaveResolved.Invoke(cascadeCount, cleared, waveSnapshot, spawns, waveActivations, animation);
                }
                maxIterations--;
            }
            return new CascadeResult(cascadeCount, totalClearedCells, hadAnyMatches, clearedPerWave, totalStats);
        }
    }
}