using System.Collections.Generic;
using Core.Board;
using Core.Contracts;
using Core.Matching;
using Core.Specials;

namespace Core.Hint
{
    public static class HintFinder
    {
        public static HintResult FindBestHint(BoardModel board)
        {
            return FindBestHint(board, HintGoalContext.Empty);
        }

        public static HintResult FindBestHint(BoardModel board, HintGoalContext goalContext)
        {
            goalContext ??= HintGoalContext.Empty;

            var snapshot = BoardSnapshotFactory.Capture(board);
            var possibleMoves = HintMoveCollector.Collect(board);

            if (possibleMoves.Count == 0)
            {
                return new HintResult(null, snapshot);
            }

            var bestMove = possibleMoves[0];
            var bestScore = HintScorer.ScoreMove(board, bestMove, goalContext);
            var bestOutcome = HintMoveOutcomeEstimator.Estimate(board, bestMove);
            var bestCompletesGoals = CompletesRemainingGoals(bestOutcome, goalContext);
            
            for (int i = 1; i < possibleMoves.Count; i++)
            {
                var move = possibleMoves[i];
                var score = HintScorer.ScoreMove(board, move, goalContext);
                var outcome = HintMoveOutcomeEstimator.Estimate(board, move);
                var completesGoals = CompletesRemainingGoals(outcome, goalContext);

                if (completesGoals && !bestCompletesGoals)
                {
                    bestScore = score;
                    bestMove = move;
                    bestCompletesGoals = completesGoals;
                }
                else if (!completesGoals && bestCompletesGoals)
                {
                    continue;
                }
                else
                {
                    if (score <= bestScore)
                    {
                        continue;
                    }
                    bestScore = score;
                    bestMove = move;
                    bestCompletesGoals = completesGoals;
                }
            }
            var explanation = BuildExplanation(board, bestMove, bestScore, goalContext);
            return new HintResult(bestMove, HintStrength.None, explanation, snapshot);
        }

        public static HintResult FindAdaptiveHint(BoardModel board, HintContext context)
        {
            return FindAdaptiveHint(board, context, HintGoalContext.Empty);
        }

        public static HintResult FindAdaptiveHint(BoardModel board, HintContext context, HintGoalContext goalContext)
        {
            goalContext ??= HintGoalContext.Empty;

            var bestHint = FindBestHint(board, goalContext);
            if (bestHint.Move == null)
            {
                return new HintResult(null, bestHint.BoardSnapshot);
            }
            
            var bestScore = bestHint.Explanation.Score;
            var strength = HintPolicy.GetStrength(context, bestScore);
            if (strength == HintStrength.None)
            {
                return new HintResult(null, bestHint.BoardSnapshot);
            }
            
            return new HintResult(bestHint.Move, strength, bestHint.Explanation, bestHint.BoardSnapshot);
        }

        private static HintExplanation BuildExplanation(BoardModel board, Move move, int bestScore, HintGoalContext goalContext)
        {
            var reasons = new List<HintReasonType>();
            if (HintMoveEvaluator.IsDirectSpecialActivation(board, move))
            {
                reasons.Add(HintReasonType.ActivatesSpecialDirectly);
            }
            else if (MoveFinder.CreatesMatch(board, move.From, move.To))
            {
                reasons.Add(HintReasonType.CreatesMatch);
                if (CreatesSpecial(board, move))
                {
                    reasons.Add(HintReasonType.CreatesSpecial);
                }
            }

            if (HintScorer.EstimateObstacleDamageCount(board, move) > 0)
            {
                reasons.Add(HintReasonType.DamagesObstacle);
            }

            var outcome = HintMoveOutcomeEstimator.Estimate(board, move);
            foreach (var pair in outcome.ClearedByColor)
            {
                if (goalContext.GetRemainingColor(pair.Key) > 0)
                {
                    reasons.Add(HintReasonType.ProgressesColorGoal);
                    break;
                }
            }
            return new HintExplanation(reasons, bestScore);
        }

        private static bool CreatesSpecial(BoardModel board, Move move)
        {
            if (!MoveFinder.CreatesMatch(board, move.From, move.To))
            {
                return false;
            }
            
            return BoardSimulation.WithTemporarySwap(board, move, () =>
            {
                var matches = MatchFinder.FindMatches(board);
                var spawns = SpecialSpawnResolver.BuildDecisions(board, matches, move);
                return spawns.Count > 0;
            });
        }

        private static bool CompletesRemainingGoals(HintMoveOutcome outcome, HintGoalContext goalContext)
        {
            if (goalContext.RemainingScore == 0 && goalContext.RemainingObstacles == 0 && goalContext.RemainingByColor.Count == 0)
            {
                return false;
            }

            foreach (var color in goalContext.RemainingByColor)
            {
                outcome.ClearedByColor.TryGetValue(color.Key, out  var predicted);
                if (predicted < color.Value)
                {
                    return false;
                }
            }

            if (outcome.EstimatedClearedPieces < goalContext.RemainingScore)
            {
                return false;
            }
            if (goalContext.RemainingObstacles > 0)
            {
                return false;
            }
            
            return true;
        }
    }
}