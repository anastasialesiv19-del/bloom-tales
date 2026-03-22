using System;
using System.Collections.Generic;
using System.Linq;
using Core.Matching;
using Core.Specials;

namespace Core.Board
{
    public static class BoardResolver
    {
        private static readonly Random _random = new Random();

        private static readonly PieceColor[] AllSpawnable =
            Enum.GetValues(typeof(PieceColor))
                .Cast<PieceColor>()
                .ToArray();

        public static int ClearCells(BoardModel board, IReadOnlyCollection<Pos> initialCells)
        {
            int clearedPiece = 0;
            foreach (Pos pos in initialCells)
                if (!board.Get(pos.Row, pos.Col).IsEmpty)
                {
                    board.Set(pos.Row, pos.Col, Piece.Empty());
                    clearedPiece++;
                }
            return clearedPiece;
        }
        
        public static int ClearMatches(BoardModel board, IReadOnlyList<Match> matches)
        {
            var uniqueCells = new HashSet<Pos>();
            foreach (var match in matches)
                foreach (var cell in match.Cells)
                    uniqueCells.Add(cell);
                
            var finalCellsToClear = SpecialEffectResolver.CollectTriggeredCells(board, uniqueCells);
            foreach (var cell in finalCellsToClear)
                board.Set(cell.Row, cell.Col, Piece.Empty());
            
            return finalCellsToClear.Count;
        }

        public static int ClearMatchesAndSpawnSpecials(BoardModel board, IReadOnlyList<Match> matches,
            IReadOnlyList<SpecialSpawnDecision> spawns)
        {
            int clearedCount = ClearMatches(board, matches);
            foreach (var spawn in spawns)
                board.Set(spawn.SpawnPosition.Row, spawn.SpawnPosition.Col, spawn.SpawnPiece);
            return clearedCount;
        }

        public static void ApplyGravity(BoardModel board)
        {
            for (int col = 0; col < board.Width; col++)
            {
                int writeRow = board.Height - 1;
                for (int row = board.Height - 1; row >= 0; row--)
                {
                    var currentPiece = board.Get(row, col);
                    if (!currentPiece.IsEmpty)
                    {
                        board.Set(writeRow, col, currentPiece);
                        writeRow--;
                    }
                }

                while (writeRow >= 0)
                {
                    board.Set(writeRow, col, Piece.Empty());
                    writeRow--;
                }
            }
        }

        public static void Refill(BoardModel board)
        {
            foreach (var pos in board.AllPositions())
            {
                if (!board.Get(pos.Row, pos.Col).IsEmpty) continue;

                var allowedIndex = AllSpawnable.Length > 0
                    ? _random.Next(AllSpawnable.Length)
                    : throw new InvalidOperationException();
                var piece = Piece.Normal(AllSpawnable[allowedIndex]);
                board.Set(pos.Row, pos.Col, piece);
            }
        }

        public static bool HasEmptyCells(BoardModel board)
        {
            foreach (Pos pos in board.AllPositions())
            {
                if (board.Get(pos.Row, pos.Col).IsEmpty)
                    return true;
            }

            return false;
        }
    }
}