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
            return ScoreMove(board, move, HintGoalContext.Empty);
        }

        public static int ScoreMove(
            BoardModel board,
            Move move,
            HintGoalContext goalContext)
        {
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
                return 0;

            goalContext ??= HintGoalContext.Empty;

            var directType = HintMoveEvaluator.GetDirectSpecialHintType(board, move);

            if (directType != DirectSpecialHintType.None)
            {
                var outcome = HintMoveOutcomeEstimator.Estimate(board, move);
                return ScoreDirectSpecialMove(directType) +
                       ScoreGoalProgress(outcome, goalContext);
            }

            if (MoveFinder.CreatesMatch(board, move.From, move.To))
                return ScoreMatchCreatingMove(board, move, goalContext);

            return 0;
        } 

        private static int ScoreMatchCreatingMove(BoardModel board, Move move, HintGoalContext goalContext)
        {
            var outcome = HintMoveOutcomeEstimator.Estimate(board, move);
            return BoardSimulation.WithTemporarySwap(board, move, () => {

                var matches = MatchFinder.FindMatches(board);
                var baseMatchScore = 0;
                foreach (var match in matches)
                    baseMatchScore += match.Cells.Count;
                var spawnBonus = GetSpawnBonus(board, matches, move);
                var goalBonus = ScoreGoalProgress(outcome, goalContext);
                return baseMatchScore + spawnBonus + goalBonus;
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
        
        private static int ScoreGoalProgress(HintMoveOutcome outcome, HintGoalContext context)
        {
            var score = 0;
            foreach (var pair in outcome.ClearedByColor)
            {
                var remaining = context.GetRemainingColor(pair.Key);
                if (remaining <= 0) continue;
                score += Math.Min(pair.Value, remaining) * 6;
            }

            if (outcome.EstimatedObstacleDamage > 0)
            {
                if (context.HasObstacleGoal)
                    score += Math.Min(outcome.EstimatedObstacleDamage, context.RemainingObstacles) * 8;
                else
                    score += outcome.EstimatedObstacleDamage * 4;
            }
            return score;
        }

        public static int EstimateObstacleDamageCount(BoardModel board, Move move)
        {
            return HintMoveOutcomeEstimator.Estimate(board, move).EstimatedObstacleDamage;
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