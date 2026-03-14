using System;

namespace Core.Board
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
            return AreAdjacent(a, b) &&  IsSwapInsideBoard(board, a, b);
        }
    }
}