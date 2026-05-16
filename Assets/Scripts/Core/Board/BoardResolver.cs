using System;
using System.Collections.Generic;
using System.Linq;
using Core.Matching;
using Core.Randomness;
using Core.Specials;


namespace Core.Board
{
    public static class BoardResolver
    {
        private static readonly PieceColor[] AllSpawnable =
            Enum.GetValues(typeof(PieceColor))
                .Cast<PieceColor>()
                .ToArray();

        private static void DamageObstaclesAffectedOrAdjacent(BoardModel board, IReadOnlyCollection<Pos> affectedCells)
        {
            var obstaclePositions = new HashSet<Pos>();

            foreach (var cell in affectedCells)
            {
                AddIfObstacle(board, cell, obstaclePositions);
                AddIfObstacle(board, new Pos(cell.Row + 1, cell.Col), obstaclePositions);
                AddIfObstacle(board, new Pos(cell.Row - 1, cell.Col), obstaclePositions);
                AddIfObstacle(board, new Pos(cell.Row, cell.Col + 1), obstaclePositions);
                AddIfObstacle(board, new Pos(cell.Row, cell.Col - 1), obstaclePositions);
            }
            
            foreach(var pos in obstaclePositions)
            {
                var obstacle = board.GetObstacle(pos.Row, pos.Col);
                obstacle.Health--;
                board.SetObstacle(pos.Row, pos.Col, obstacle.Health <= 0 ? ObstacleModel.Empty : obstacle);
            }
        }

        private static void AddIfObstacle(BoardModel board, Pos pos, HashSet<Pos> obstaclePositions)
        {
            if (!board.InBounds(pos.Row, pos.Col))
                return;
            if(board.HasObstacle(pos.Row, pos.Col))
                obstaclePositions.Add(pos);
        }
        
        public static int ClearCells(BoardModel board, IReadOnlyCollection<Pos> initialCells)
        {
            var affectedCells = new HashSet<Pos>(initialCells);

            DamageObstaclesAffectedOrAdjacent(board, affectedCells);

            int clearedPiece = 0;

            foreach (var pos in affectedCells)
            {
                if (board.HasObstacle(pos.Row, pos.Col))
                    continue;

                if (!board.Get(pos.Row, pos.Col).IsEmpty)
                {
                    board.Set(pos.Row, pos.Col, Piece.Empty());
                    clearedPiece++;
                }
            }

            return clearedPiece;
        }
        
        public static int ClearMatches(BoardModel board, IReadOnlyList<Match> matches)
        {
            var uniqueCells = new HashSet<Pos>();

            foreach (var match in matches)
            {
                foreach (var cell in match.Cells)
                {
                    uniqueCells.Add(cell);
                }
            }

            var finalCellsToClear = SpecialEffectResolver.CollectTriggeredCells(board, uniqueCells);

            return ClearCells(board, finalCellsToClear);
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
                    var currentPos = new Pos(row, col);

                    if (board.HasObstacle(currentPos.Row, currentPos.Col))
                    {
                        board.Set(row, col, Piece.Empty());
                        writeRow = row - 1;
                        continue;
                    }

                    var currentPiece = board.Get(row, col);

                    if (currentPiece.IsEmpty)
                        continue;

                    while (writeRow >= 0 && board.HasObstacle(writeRow, col))
                    {
                        writeRow--;
                    }

                    if (writeRow != row)
                    {
                        board.Set(writeRow, col, currentPiece);
                        board.Set(row, col, Piece.Empty());
                    }

                    writeRow--;
                }

                while (writeRow >= 0)
                {
                    var pos = new Pos(writeRow, col);

                    if (!board.HasObstacle(pos.Row, pos.Col))
                        board.Set(writeRow, col, Piece.Empty());

                    writeRow--;
                }
            }
        }

        public static void Refill(BoardModel board, IRandomProvider provider = null)
        {
            var rng = provider ?? new SystemRandomProvider();
            
            foreach (var pos in board.AllPositions())
            {
                if (board.HasObstacle(pos.Row, pos.Col))
                    continue;
                
                if (!board.Get(pos.Row, pos.Col).IsEmpty) continue;

                var allowedIndex = AllSpawnable.Length > 0
                    ? rng.Next(AllSpawnable.Length)
                    : throw new InvalidOperationException();
                var piece = Piece.Normal(AllSpawnable[allowedIndex]);
                board.Set(pos.Row, pos.Col, piece);
            }
        }

        public static bool HasEmptyCells(BoardModel board)
        {
            return board.AllPositions().Any(pos => !board.HasObstacle(pos.Row, pos.Col) && board.Get(pos.Row, pos.Col).IsEmpty);
        }
        
        
    }
}