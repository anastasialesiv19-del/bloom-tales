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

        public TurnResult(bool wasSwapValid, bool wasApplied, bool wasShuffled, bool createdMatch,bool boardWasPlayableBeforeMove, int cascadeCount,
            int totalClearedCells, Move move,  FailureReason failureReason)
        {
            WasSwapValid = wasSwapValid;
            WasApplied = wasApplied;
            WasShuffled = wasShuffled;
            CreatedMatch = createdMatch;
            BoardWasPlayableBeforeMove = boardWasPlayableBeforeMove;
            CascadeCount = cascadeCount;
            TotalClearedCells = totalClearedCells;
            Move = move;
            FailureReason = failureReason;
        }
    }

    public enum FailureReason
    {
        None,
        InvalidSwap,
        NoMatchCreated
    }
}