using System;
using Core.Matching;

namespace Core.Turns
{
    public class TurnResult
    {
        public bool WasSwapValid { get; }
        public bool WasApplied { get; }
        public bool WasShuffled { get; }
        public bool CreatedMatch { get; }
        public bool BoardWasPlayableBeforeMove { get; }
        public int CascadeCount { get; }
        public int TotalClearedCells { get; }
        public Move Move { get; }
        public FailureReason FailureReason { get; }

        public TurnTrace Trace { get; }

        public TurnResult(bool wasSwapValid, bool wasApplied, bool wasShuffled, bool createdMatch,
            bool boardWasPlayableBeforeMove, int cascadeCount,
            int totalClearedCells, Move move, FailureReason failureReason, TurnTrace trace)
        {
            if (failureReason != FailureReason.None)
            {
                if (wasApplied)
                    throw new ArgumentException("Failed turn cannot be applied.");
                if (cascadeCount != 0)
                    throw new ArgumentException("Failed turn cannot have cascades.");
                if (totalClearedCells != 0)
                    throw new ArgumentException("Failed turn cannot clear cells.");
            }

            if (wasShuffled && !wasApplied)
                throw new ArgumentException("Shuffled turn must be applied.");
            if (createdMatch && !wasApplied)
                throw new ArgumentException("Turn with created match must be applied.");
            
            WasSwapValid = wasSwapValid;
            WasApplied = wasApplied;
            WasShuffled = wasShuffled;
            CreatedMatch = createdMatch;
            BoardWasPlayableBeforeMove = boardWasPlayableBeforeMove;
            CascadeCount = cascadeCount;
            TotalClearedCells = totalClearedCells;
            Move = move;
            FailureReason = failureReason;
            Trace = trace;
        }
    }

    public enum FailureReason
    {
        None,
        InvalidSwap,
        NoMatchCreated
    }
}