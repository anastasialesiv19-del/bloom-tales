using System;
using System.Collections.Generic;
using System.Linq;
using Core.Matching;
using Core.Specials;

namespace Core.Hint
{
    public static class HintScorer
    {
        public static int ScoreMove(BoardModel board, Move move)
        {
            if (MoveFinder.CreatesMatch(board, move.From, move.To))
                return ScoreMatchCreatingMove(board, move);

            var type = HintMoveEvaluator.GetDirectSpecialHintType(board, move);
            if (type != DirectSpecialHintType.None)
                return ScoreDirectSpecialMove(type);

            return 0;
        }

        private static int ScoreMatchCreatingMove(BoardModel board, Move move)
        {
            board.Swap(move.From, move.To);
            var matches = MatchFinder.FindMatches(board);
            int baseMatchScore = 0;
            foreach (var match in matches)
                baseMatchScore += match.Cells.Count;
            var spawnBonus = GetSpawnBonus(board, matches, move);
            board.Swap(move.From, move.To);
            return baseMatchScore + spawnBonus;
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