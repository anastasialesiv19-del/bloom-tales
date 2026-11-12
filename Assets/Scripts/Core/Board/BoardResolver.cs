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
        private static readonly PieceColor[] AllSpawnable = Enum.GetValues(typeof(PieceColor)).Cast<PieceColor>().ToArray();

        private static int DamageObstaclesAffectedOrAdjacent(BoardModel board, IReadOnlyCollection<Pos> affectedCells)
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
            
            var destroyed = 0;
            foreach (var pos in obstaclePositions)
            {
                var obstacle = board.GetObstacle(pos.Row, pos.Col);
                obstacle.Health--;
                if (obstacle.Health <= 0)
                {
                    board.SetObstacle(pos.Row, pos.Col, ObstacleModel.Empty);
                    destroyed++;
                }
                else
                {
                    board.SetObstacle(pos.Row, pos.Col, obstacle); 
                }
            }
            return destroyed;
        }

        private static void AddIfObstacle(BoardModel board, Pos pos, HashSet<Pos> obstaclePositions)
        {
            if (!board.InBounds(pos.Row, pos.Col))
            {
                return;
            }
            if (board.HasObstacle(pos.Row, pos.Col))
            {
                obstaclePositions.Add(pos); 
            }
        }
        
        private static void AddColorCount(Dictionary<PieceColor, int> clearedByColor, Piece piece)
        {
            if (piece.Color == null)
            {
                return;
            }
                
            var color = piece.Color.Value;
            if (clearedByColor.ContainsKey(color))
            {
                clearedByColor[color]++;
            }
            else
            {
                clearedByColor[color] = 1;
            }
        }
        
        
        public static ClearResolutionStats ClearCells(BoardModel board, IReadOnlyCollection<Pos> initialCells)
        {
            var affectedCells = new HashSet<Pos>(initialCells);
            var destroyedObstacles = DamageObstaclesAffectedOrAdjacent(board, affectedCells);
            var clearedPieces = 0;
            var clearedByColor = new Dictionary<PieceColor, int>();
            
            foreach (var pos in affectedCells)
            {
                if (board.HasObstacle(pos.Row, pos.Col))
                {
                    continue;
                }
                
                var piece = board.Get(pos.Row, pos.Col);
                if (piece.IsEmpty)
                {
                    continue;
                }
                    
                AddColorCount(clearedByColor, piece);
                board.Set(pos.Row, pos.Col, Piece.Empty());
                clearedPieces++;
            }
            return new ClearResolutionStats(clearedPieces, clearedByColor, destroyedObstacles);
        }
        
        public static ClearResolutionStats ClearMatches(BoardModel board, IReadOnlyList<Match> matches)
        {
            return ClearMatches(board, matches, out _);
        }

        public static ClearResolutionStats ClearMatches(BoardModel board, IReadOnlyList<Match> matches, out IReadOnlyList<SpecialActivationEvent> activations)
        {
            var uniqueCells = new HashSet<Pos>();
            foreach (var match in matches)
            {
                foreach (var cell in match.Cells)
                {
                    uniqueCells.Add(cell);
                }
            }
                
            var finalCellsToClear = SpecialEffectResolver.CollectTriggeredCells(board, uniqueCells, out activations);
            return ClearCells(board, finalCellsToClear);
        }
        
        public static ClearResolutionStats ClearMatchesAndSpawnSpecials(BoardModel board, IReadOnlyList<Match> matches, IReadOnlyList<SpecialSpawnDecision> spawns)
        {
            return ClearMatchesAndSpawnSpecials(board, matches, spawns, out _);
        }

        public static ClearResolutionStats ClearMatchesAndSpawnSpecials(BoardModel board, IReadOnlyList<Match> matches,
            IReadOnlyList<SpecialSpawnDecision> spawns, out IReadOnlyList<SpecialActivationEvent> activations)
        {
            var stats = ClearMatches(board, matches, out activations);

            foreach (var spawn in spawns)
            {
                board.Set(
                    spawn.SpawnPosition.Row,
                    spawn.SpawnPosition.Col,
                    spawn.SpawnPiece);
            }
            return stats;
        }

        public static IReadOnlyList<PieceFall> ApplyGravity(BoardModel board)
        {
            var pieceFallList = new  List<PieceFall>();
            for (var col = 0; col < board.Width; col++)
            {
                var writeRow = board.Height - 1;
                for (var row = board.Height - 1; row >= 0; row--)
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
                    {
                        continue;
                    }
                    
                    while (writeRow >= 0 && board.HasObstacle(writeRow, col))
                    {
                        writeRow--;
                    }

                    if (writeRow != row)
                    {
                        board.Set(writeRow, col, currentPiece);
                        board.Set(row, col, Piece.Empty());
                        pieceFallList.Add(new PieceFall(new Pos(row, col), new Pos(writeRow, col)));
                    }
                    writeRow--;
                }

                while (writeRow >= 0)
                {
                    var pos = new Pos(writeRow, col);
                    if (!board.HasObstacle(pos.Row, pos.Col))
                    {
                        board.Set(writeRow, col, Piece.Empty());
                    }
                    writeRow--;
                }
            }
            return pieceFallList;
        }

        public static IReadOnlyList<Pos> Refill(BoardModel board, IRandomProvider provider = null)
        {
            var rng = provider ?? new SystemRandomProvider();
            var spawnPieces =  new List<Pos>();
            
            foreach (var pos in board.AllPositions())
            {
                if (board.HasObstacle(pos.Row, pos.Col))
                {
                    continue;
                }
                if (!board.Get(pos.Row, pos.Col).IsEmpty)
                {
                    continue;
                }

                var allowedIndex = AllSpawnable.Length > 0 ? rng.Next(AllSpawnable.Length) : throw new InvalidOperationException();
                var piece = Piece.Normal(AllSpawnable[allowedIndex]);
                board.Set(pos.Row, pos.Col, piece);
                spawnPieces.Add(pos);
            }
            return spawnPieces;
        }

        public static bool HasEmptyCells(BoardModel board)
        {
            return board.AllPositions().Any(pos => !board.HasObstacle(pos.Row, pos.Col) && board.Get(pos.Row, pos.Col).IsEmpty);
        }
    }
}