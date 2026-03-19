using Core.Matching;

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

        public static HintResult FindAdaptiveHint(BoardModel board, HintContext context)
        {
            var strength = HintPolicy.GetStrength(context);
            if(strength ==  HintStrength.None) return new HintResult(null, HintStrength.None);
            var result = FindBestHint(board);
            return new HintResult(result.Move, strength);
        }
    }
}