using System;

namespace Core.Matching
{
    public static class MoveValidator
    {
        public static bool AreAdjacent(Pos a, Pos b)
        {
            int dx = Math.Abs(a.Row - b.Row);
            int dy = Math.Abs(a.Col - b.Col);
            return dx + dy == 1;
        }

        public static bool IsSwapInsideBoard(BoardModel board, Pos a, Pos b)
        {
            return board.InBounds(a.Row, a.Col) && board.InBounds(b.Row, b.Col);
        }

        public static bool IsValidSwap(BoardModel board, Pos a, Pos b)
        {
            if (!IsSwapInsideBoard(board, a, b))
                return false;

            if (!AreAdjacent(a, b))
                return false;

            if (board.HasObstacle(a.Row, a.Col) || board.HasObstacle(b.Row, b.Col))
                return false;

            return true;
        }
    }
}