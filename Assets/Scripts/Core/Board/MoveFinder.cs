using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Matching
{
    public static class MoveFinder
    {
        public static bool CreatesMatch(BoardModel board, Pos a, Pos b)
        {
            bool isValidSwap = MoveValidator.IsValidSwap(board, a, b);
            if (!isValidSwap) return false;
            board.Swap(a, b);
            try
            {
                return MatchFinder.FindMatches(board).Count > 0;
            }
            finally
            {
                board.Swap(a, b);
            }
        }

        public static List<Move> FindAllPossibleMoves(BoardModel board)
        {
            List<Move> moves = new List<Move>();
            foreach (var pos in board.AllPositions())
            {
                if (pos.Col + 1 < board.Width)
                {
                    var rightNeighbor =  new Pos(pos.Row, pos.Col + 1);
                    if (CreatesMatch(board, pos, rightNeighbor))
                        moves.Add(new Move(pos, rightNeighbor));
                    
                }
                
                if (pos.Row + 1 < board.Height)
                {
                    var downNeighbor = new Pos(pos.Row + 1, pos.Col);
                    if (CreatesMatch(board, pos, downNeighbor))
                        moves.Add(new Move(pos, downNeighbor));
                }
            }
            return moves;
        }

        public static bool HasAnyPossibleMove(BoardModel board)
        {
            foreach (var pos in board.AllPositions())
            {
                if (pos.Col + 1 < board.Width)
                {
                    var rightNeighbor =  new Pos(pos.Row, pos.Col + 1);
                    if (CreatesMatch(board, pos, rightNeighbor))
                        return true;

                }
                
                if (pos.Row + 1 < board.Height)
                {
                    var downNeighbor = new Pos(pos.Row + 1, pos.Col);
                    if (CreatesMatch(board, pos, downNeighbor))
                        return true;
                }
            }
            return false;
        }

        public static bool IsDeadlocked(BoardModel board)
        {
            return !HasAnyPossibleMove(board);
        }
    }
}