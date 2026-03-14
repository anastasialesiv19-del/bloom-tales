using System;
using System.Collections.Generic;
using System.Linq;
using Core.Matching;

namespace Core.Board
{
    public static class BoardResolver
    {
        private static Random _random = new Random();
        private static readonly PieceType[] AllSpawnable =
            Enum.GetValues(typeof(PieceType))
                .Cast<PieceType>()
                .Where(t => t != PieceType.Empty)
                .ToArray();
        
        public static int ClearMatches(BoardModel board, List<Match> matches)
        {
            var uniqueCells = new HashSet<Pos>();
            foreach (var match in matches)
                foreach (var cell in match.Cells)
                    uniqueCells.Add(cell);
            
            foreach (var cell in uniqueCells)
                board.Set(cell.Row, cell.Col, PieceType.Empty);
            return uniqueCells.Count;
        }

        public static void ApplyGravity(BoardModel board)
        {
            for (int col = 0; col < board.Width; col++)
            {
                int writeRow = board.Height - 1;
                for (int row = board.Height - 1; row >= 0; row--)
                {
                    var currentType = board.Get(row, col);
                    if (currentType != PieceType.Empty)
                    {
                        board.Set(writeRow, col, currentType);
                        writeRow--;
                    }
                }

                while (writeRow >= 0)
                {
                    board.Set(writeRow, col, PieceType.Empty);
                    writeRow--;
                }
                
            }
        }

        public static void Refill(BoardModel board)
        {
            foreach (var pos in board.AllPositions())
            {
                if (board.Get(pos.Row, pos.Col) == PieceType.Empty)
                {
                    var allowedIndex = AllSpawnable.Length > 0 ? _random.Next(AllSpawnable.Length) : throw new InvalidOperationException();

                    board.Set(pos.Row, pos.Col, AllSpawnable[allowedIndex]);
                }
            }
        }

        public static bool HasEmptyCells(BoardModel board)
        {
            foreach (Pos pos in board.AllPositions())
            {
                if(board.Get(pos.Row, pos.Col) == PieceType.Empty)
                    return true;
            }
            return false;
        }
    }
}