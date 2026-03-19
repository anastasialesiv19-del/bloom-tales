using Core.Generation;

namespace Core.Matching
{
    public static class TurnProcessor
    {
        public static TurnResult TryProcessMove(BoardModel board, Move move)
        {
            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
                return new TurnResult(false, false, false, false, isPlayable,
                    0, 0, move, FailureReason.InvalidSwap);

            if (!MoveFinder.CreatesMatch(board, move.From, move.To))
                return new TurnResult(true, false, false, false, isPlayable,
                    0, 0, move, FailureReason.NoMatchCreated);

            board.Swap(move.From, move.To);
            var cascadeResult = CascadeResolver.Resolve(board);
            bool wasShuffled = false;
            if (!BoardShuffler.IsPlayableBoard(board))
            {
                BoardShuffler.Shuffle(board);
                wasShuffled = true;
            }
                
            
            return new TurnResult(true, true, wasShuffled, true, isPlayable,
                cascadeResult.CascadeCount, cascadeResult.TotalClearedCells, 
                move, FailureReason.None );
        }
    }
}