using System;
using System.Collections.Generic;
using System.Linq;
using Core.Board;

namespace Core.Hint
{
    public static class HintMoveCollector
    {
        public static List<Move> Collect(BoardModel board)
        {
            var result = new HashSet<Move>();
            var matchCreatingMove = MoveFinder.FindAllPossibleMoves(board);
            result.UnionWith(matchCreatingMove);
            var specialActivationMove = FindAllDirectSpecialHintMoves(board);
            result.UnionWith(specialActivationMove);

            return result.ToList();
        }

        private static List<Move> FindAllDirectSpecialHintMoves(BoardModel board)
        {
            var moves = new List<Move>();
            foreach (var pos in board.AllPositions())
            {
                if (pos.Col + 1 < board.Width)
                {
                    var rightNeighbor = new Pos(pos.Row, pos.Col + 1);
                    if (!MoveValidator.IsValidSwap(board, pos, rightNeighbor))
                    {
                        continue;
                    }
                    var move = new Move(pos, rightNeighbor);
                    if (HintMoveEvaluator.IsDirectSpecialActivation(board, move))
                    {
                        moves.Add(move);
                    }
                }
                
                if (pos.Row + 1 < board.Height)
                {
                    var downNeighbor = new Pos(pos.Row + 1, pos.Col);
                    if (!MoveValidator.IsValidSwap(board, pos, downNeighbor))
                    {
                        continue;
                    }
                    var move = new Move(pos, downNeighbor);
                    if (HintMoveEvaluator.IsDirectSpecialActivation(board, move))
                    {
                        moves.Add(move);
                    }
                }
            }
            return moves;
        }
    }
}