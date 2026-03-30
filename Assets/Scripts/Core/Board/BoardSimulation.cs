using System;
using Core.Matching;

namespace Core.Board
{
    public static class BoardSimulation
    {
        public static T WithTemporarySwap<T>(BoardModel board, Move move, Func<T> action)
        {
            board.Swap(move.From, move.To);
            try
            {
                return action();
            }
            finally
            {
                board.Swap(move.From, move.To);
            }
        }
    }
}