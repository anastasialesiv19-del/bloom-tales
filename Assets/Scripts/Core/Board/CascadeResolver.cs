using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Board
{
    public static class CascadeResolver
    {
        public static CascadeResult Resolve(BoardModel board)
        {
            int cascadeCount = 0;
            int cleared = 0;
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
                cleared = BoardResolver.ClearMatches(board,  matches);
                BoardResolver.ApplyGravity(board);
                BoardResolver.Refill(board);
                
                cascadeCount++;
                totalClearedCells += cleared;
                clearedPerWave.Add(cleared);
                maxIterations--;
            }
            return new CascadeResult(cascadeCount,totalClearedCells, hadAnyMatches, clearedPerWave);
        }
    }
}