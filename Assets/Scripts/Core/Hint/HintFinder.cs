using Core.Board;

namespace Core.Hint
{
    public static class HintFinder
    {
        public static HintResult FindHint(BoardModel board)
        {
            var possibleMoves = MoveFinder.FindAllPossibleMoves(board);
            if (possibleMoves.Count == 0) return new HintResult(false, null);
            
            return new HintResult(true, possibleMoves[0]);
        }
    }
}