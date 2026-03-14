namespace Core.Board
{
    public static class TurnProcessor
    {
        public static TurnResult TryProcessMove(BoardModel board, Move move)
        {
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
                return new TurnResult(false, false, false, 0, 0, move, FailureReason.InvalidSwap);

            if (!MoveFinder.CreatesMatch(board, move.From, move.To))
                return new TurnResult(true, false, false, 0, 0, move, FailureReason.NoMatchCreated);

            board.Swap(move.From, move.To);
            var cascadeResult = CascadeResolver.Resolve(board);
            return new TurnResult(true, true, true, 
                cascadeResult.CascadeCount, 
                cascadeResult.TotalClearedCells, move, FailureReason.None );
        }
    }
}