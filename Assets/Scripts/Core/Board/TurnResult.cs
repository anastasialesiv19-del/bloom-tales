using JetBrains.Annotations;

namespace Core.Board
{
    public class TurnResult
    {
        public bool WasSwapValid { get; }
        public bool WasApplied { get; }
        public bool CreatedMatch { get; }
        public int CascadeCount { get; }
        public int TotalClearedCells { get; }
        public Move Move { get; }
        public FailureReason FailureReason { get; }

        public TurnResult(bool wasSwapValid, bool wasApplied, bool createdMatch, int cascadeCount,
            int totalClearedCells, Move move,  FailureReason failureReason)
        {
            WasSwapValid = wasSwapValid;
            WasApplied = wasApplied;
            CreatedMatch = createdMatch;
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