using System;
using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Matching;
using Core.Specials;

namespace Core.Hint
{
    public static class HintScorer
    {
        public static int ScoreMove(BoardModel board, Move move)
        {
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
                return 0;

            if (MoveFinder.CreatesMatch(board, move.From, move.To))
                return ScoreMatchCreatingMove(board, move);

            var type = HintMoveEvaluator.GetDirectSpecialHintType(board, move);

            if (type != DirectSpecialHintType.None)
            {
                var obstacleBonus = EstimateObstacleDamageCount(board, move) * 4;
                return ScoreDirectSpecialMove(type) + obstacleBonus;
            }

            return 0;
        }

        private static int ScoreMatchCreatingMove(BoardModel board, Move move)
        {
            return BoardSimulation.WithTemporarySwap(board, move, () => {

                var matches = MatchFinder.FindMatches(board);
                var baseMatchScore = 0;
                foreach (var match in matches)
                    baseMatchScore += match.Cells.Count;
                var spawnBonus = GetSpawnBonus(board, matches, move);
                var obstacleBonus = EstimateObstacleDamageCount(board, move) * 4;
                return baseMatchScore + spawnBonus + obstacleBonus;
            });
        }

        private static int GetSpawnBonus(BoardModel board, List<Match> matches, Move move)
        {
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches, move);
            if (spawns.Count == 0) return 0;

            int maxBonus = 0;
            foreach (var spawn in spawns)
            {
                var kind = spawn.SpawnPiece.Kind;
                maxBonus = Math.Max(maxBonus, GetSpawnKindBonus(kind));
            }
            
            return maxBonus;
        }
        private static int EstimateMatchMoveObstacleDamage(BoardModel board, Move move)
        {
            return BoardSimulation.WithTemporarySwap(board, move, () =>
            {
                var matches = MatchFinder.FindMatches(board);
                var matchedCells = new HashSet<Pos>();

                foreach (var match in matches)
                {
                    foreach (var cell in match.Cells)
                        matchedCells.Add(cell);
                }

                var finalCellsToClear = SpecialEffectResolver.CollectTriggeredCells(board, matchedCells);

                return CountAffectedObstacles(board, finalCellsToClear);
            });
        }

        public static int EstimateObstacleDamageCount(BoardModel board, Move move)
        {
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
                return 0;

            if (MoveFinder.CreatesMatch(board, move.From, move.To))
                return EstimateMatchMoveObstacleDamage(board, move);

            var type = HintMoveEvaluator.GetDirectSpecialHintType(board, move);

            if (type == DirectSpecialHintType.None)
                return 0;

            return EstimateDirectSpecialObstacleDamage(board, move, type);
        }
        private static int EstimateDirectSpecialObstacleDamage(
            BoardModel board,
            Move move,
            DirectSpecialHintType type)
        {
            if (type == DirectSpecialHintType.ColorBombWithNormal ||
                type == DirectSpecialHintType.ColorBombWithSpecial ||
                type == DirectSpecialHintType.ColorBombWithColorBomb)
            {
                var initialCells = new HashSet<Pos> { move.From, move.To };
                var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, initialCells);
                return CountAffectedObstacles(board, finalCells);
            }

            if (type == DirectSpecialHintType.LineOrAreaWithNormal)
                return EstimateLineOrAreaWithNormalObstacleDamage(board, move);

            return EstimateSpecialComboObstacleDamage(board, move);
        }
        
        private static int EstimateLineOrAreaWithNormalObstacleDamage(BoardModel board, Move move)
        {
            return BoardSimulation.WithTemporarySwap(board, move, () =>
            {
                var pieceAtFrom = board.Get(move.From.Row, move.From.Col);
                var pieceAtTo = board.Get(move.To.Row, move.To.Col);

                Pos specialFinalCell;

                if (IsLineOrArea(pieceAtFrom.Kind))
                    specialFinalCell = move.From;
                else if (IsLineOrArea(pieceAtTo.Kind))
                    specialFinalCell = move.To;
                else
                    return 0;

                var specialPiece = board.Get(specialFinalCell.Row, specialFinalCell.Col);

                var effectCells = GetDirectSpecialEffectCells(
                    board,
                    specialFinalCell,
                    specialPiece.Kind).ToHashSet();

                var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, effectCells);

                return CountAffectedObstacles(board, finalCells);
            });
        }
        
        private static int EstimateSpecialComboObstacleDamage(BoardModel board, Move move)
        {
            var pieceA = board.Get(move.From.Row, move.From.Col);
            var pieceB = board.Get(move.To.Row, move.To.Col);

            var cells = new HashSet<Pos>();

            if (IsLineOrArea(pieceA.Kind))
                cells.UnionWith(GetDirectSpecialEffectCells(board, move.From, pieceA.Kind));

            if (IsLineOrArea(pieceB.Kind))
                cells.UnionWith(GetDirectSpecialEffectCells(board, move.To, pieceB.Kind));

            var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, cells);

            return CountAffectedObstacles(board, finalCells);
        }
        
        private static int CountAffectedObstacles(BoardModel board, IReadOnlyCollection<Pos> clearedCells)
        {
            var affectedObstacles = new HashSet<Pos>();

            foreach (var cell in clearedCells)
            {
                AddObstacleIfPresent(board, cell, affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row + 1, cell.Col), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row - 1, cell.Col), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row, cell.Col + 1), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row, cell.Col - 1), affectedObstacles);
            }

            return affectedObstacles.Count;
        }

        private static void AddObstacleIfPresent(BoardModel board, Pos pos, HashSet<Pos> affectedObstacles)
        {
            if (!board.InBounds(pos.Row, pos.Col))
                return;

            if (board.HasObstacle(pos.Row, pos.Col))
                affectedObstacles.Add(pos);
        }
        private static int GetSpawnKindBonus(PieceKind kind)
        {
            switch (kind)
            {
                case PieceKind.ColorBomb:
                    return 7;
                case PieceKind.AreaBomb:
                    return 5;
                case PieceKind.RowClear:
                case PieceKind.ColumnClear:
                    return 3;
            }
            return 0;
        }

        private static IEnumerable<Pos> GetDirectSpecialEffectCells(
            BoardModel board,
            Pos cell,
            PieceKind kind)
        {
            if (kind == PieceKind.RowClear)
            {
                for (int col = 0; col < board.Width; col++)
                    yield return new Pos(cell.Row, col);

                yield break;
            }

            if (kind == PieceKind.ColumnClear)
            {
                for (int row = 0; row < board.Height; row++)
                    yield return new Pos(row, cell.Col);

                yield break;
            }

            if (kind == PieceKind.AreaBomb)
            {
                for (int row = cell.Row - 1; row <= cell.Row + 1; row++)
                {
                    for (int col = cell.Col - 1; col <= cell.Col + 1; col++)
                    {
                        if (board.InBounds(row, col))
                            yield return new Pos(row, col);
                    }
                }

                yield break;
            }
        }
        private static bool IsLineOrArea(PieceKind kind)
        {
            return kind == PieceKind.RowClear ||
                   kind == PieceKind.ColumnClear ||
                   kind == PieceKind.AreaBomb;
        }
        private static int ScoreDirectSpecialMove(DirectSpecialHintType type)
        {
            switch (type)
            {
                case DirectSpecialHintType.LineOrAreaWithNormal:
                    return 5;
                case DirectSpecialHintType.LineWithLine:
                    return 7;
                case DirectSpecialHintType.LineWithArea:
                    return 8;
                case DirectSpecialHintType.AreaWithArea:
                    return 9;
                case DirectSpecialHintType.ColorBombWithNormal:
                    return 10;
                case DirectSpecialHintType.ColorBombWithSpecial:
                    return 12;
                case DirectSpecialHintType.ColorBombWithColorBomb:
                    return 15;
                default:
                    return 0;
            }
        }
    }
}