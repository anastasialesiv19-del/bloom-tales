using System;
using System.Collections.Generic;
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
                return ScoreDirectSpecialMove(type);

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
                var obstacleBonus = GetObstacleDamageBonus(board, matches);
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

        private static int GetObstacleDamageBonus(BoardModel board, List<Match> matches)
        {
            var matchedCells = new HashSet<Pos>();

            foreach (var match in matches)
            {
                foreach (var cell in match.Cells)
                    matchedCells.Add(cell);
            }

            var finalCellsToClear = SpecialEffectResolver.CollectTriggeredCells(board, matchedCells);
            var affectedObstacles = new HashSet<Pos>();

            foreach (var cell in finalCellsToClear)
            {
                AddObstacleIfPresent(board, cell, affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row + 1, cell.Col), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row - 1, cell.Col), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row, cell.Col + 1), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row, cell.Col - 1), affectedObstacles);
            }

            return affectedObstacles.Count * 4;
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