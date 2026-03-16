using Core.Board;

namespace Core.Hint
{
    public static class HintFinder
    {
        public static HintResult FindBestHint(BoardModel board)
        {
            var possibleMoves = MoveFinder.FindAllPossibleMoves(board);
            if (possibleMoves.Count == 0) return new HintResult(null);

            int bestScore = -1;
            Move? bestMove = null;
            foreach (var move in possibleMoves)
            {
                int score = HintScorer.ScoreMove(board, move);
                
                if (score <= bestScore) continue;
                
                bestScore = score;
                bestMove = move;
            }
            return new HintResult(bestMove);
        }
    }
}