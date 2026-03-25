using Core.Matching;

namespace Core.Hint
{
    public static class HintScorer
    {
        public static int ScoreMove(BoardModel board, Move move)
        {
            if(MoveFinder.CreatesMatch(board, move.From, move.To))
                return ScoreMatchCreatingMove(board, move);
            
            var type = HintMoveEvaluator.GetDirectSpecialHintType(board, move);
            if (type != DirectSpecialHintType.None)
                return ScoreDirectSpecialMove(type);

            return 0;
        }

        private static int ScoreMatchCreatingMove(BoardModel board, Move move)
        {
            board.Swap(move.From, move.To);
            var matches = MatchFinder.FindMatches(board);
            int cells = 0;
            foreach (var match in matches)
                cells += match.Cells.Count;
            board.Swap(move.From, move.To);
            return cells;
        }
        
        private static int ScoreDirectSpecialMove(DirectSpecialHintType type)
        {
            switch (type)
            {
                case DirectSpecialHintType.LineOrAreaWithNormal:
                    return 5;
                case DirectSpecialHintType.LineWithLine:
                    return 7;
                case DirectSpecialHintType.LineWithArea:
                    return 8;
                case DirectSpecialHintType.AreaWithArea:
                    return 9;
                case DirectSpecialHintType.ColorBombWithNormal:
                    return 10;
                case DirectSpecialHintType.ColorBombWithSpecial:
                    return 12;
                case DirectSpecialHintType.ColorBombWithColorBomb:
                    return 15;
                default:
                    return 0;
            }
        }
    }
}