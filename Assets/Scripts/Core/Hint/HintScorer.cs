using Core.Matching;

namespace Core.Hint
{
    public static class HintScorer
    {
        public static int ScoreMove(BoardModel board, Move move)
        {
            if (HintMoveEvaluator.IsDirectSpecialActivation(board, move)) return 5;
            
            board.Swap(move.From, move.To);
            var matches = MatchFinder.FindMatches(board);
            int cells = 0;
            foreach (var match in matches)
                cells += match.Cells.Count;
            board.Swap(move.From, move.To);
            return cells;
        }
    }
}