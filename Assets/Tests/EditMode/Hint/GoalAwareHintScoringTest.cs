using System.Collections.Generic;
using System.Linq;
using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class GoalAwareHintScoringTest
    {
        [Test]
        public void HintScorer_WhenMoveClearsGoalColor_AddsColorGoalBonus()
        {
            var board = CreateBoardWithRedMove();
            var move = new Move(new Pos(0, 1), new Pos(1, 1));

            var scoreWithoutGoal = HintScorer.ScoreMove(board, move);

            var context = new HintGoalContext(
                new Dictionary<PieceColor, int>
                {
                    { PieceColor.Red, 5 }
                },
                0);

            var scoreWithGoal = HintScorer.ScoreMove(board, move, context);

            Assert.Greater(scoreWithGoal, scoreWithoutGoal);
        }

        [Test]
        public void HintScorer_WhenGoalColorAlreadyCompleted_DoesNotAddColorGoalBonus()
        {
            var board = CreateBoardWithRedMove();
            var move = new Move(new Pos(0, 1), new Pos(1, 1));

            var scoreWithoutGoal = HintScorer.ScoreMove(board, move);

            var context = new HintGoalContext(
                new Dictionary<PieceColor, int>
                {
                    { PieceColor.Red, 0 }
                },
                0);

            var scoreWithCompletedGoal = HintScorer.ScoreMove(board, move, context);

            Assert.AreEqual(scoreWithoutGoal, scoreWithCompletedGoal);
        }

        [Test]
        public void HintExplanation_WhenBestMoveClearsGoalColor_IncludesProgressesColorGoalReason()
        {
            var board = CreateBoardWithRedMove();

            var context = new HintGoalContext(
                new Dictionary<PieceColor, int>
                {
                    { PieceColor.Red, 5 }
                },
                0);

            var hint = HintFinder.FindBestHint(board, context);

            Assert.IsNotNull(hint.Explanation);
            Assert.IsTrue(hint.Explanation.Reasons.Contains(HintReasonType.ProgressesColorGoal));
        }

        [Test]
        public void HintScorer_WhenClearExceedsRemainingColorGoal_CapsGoalBonus()
        {
            var board = CreateBoardWithRedMove();
            var move = new Move(new Pos(0, 1), new Pos(1, 1));
            
            var estimate = HintMoveOutcomeEstimator.Estimate(board, move);
            Assert.AreEqual(3, estimate.ClearedByColor[PieceColor.Red]);
            
            var scoreWithoutGoal = HintScorer.ScoreMove(board, move);
            
            var contextWithOneRemaining = new HintGoalContext(
                new Dictionary<PieceColor, int>
                {
                    { PieceColor.Red, 1 }
                },
                0);
            var contextWithThreeRemaining = new HintGoalContext(
                new Dictionary<PieceColor, int>
                {
                    { PieceColor.Red, 3 }
                },
                0);
            var contextWithFiveRemaining = new HintGoalContext(
                new Dictionary<PieceColor, int>
                {
                    { PieceColor.Red, 5 }
                },
                0);

            var scoreWithOneRemaining = HintScorer.ScoreMove(board, move, contextWithOneRemaining);
            var scoreWithThreeRemaining = HintScorer.ScoreMove(board, move, contextWithThreeRemaining);
            var scoreWithFiveRemaining = HintScorer.ScoreMove(board, move, contextWithFiveRemaining);
            
            Assert.AreEqual(6, scoreWithOneRemaining - scoreWithoutGoal);
            Assert.AreEqual(18, scoreWithThreeRemaining - scoreWithoutGoal);
            Assert.AreEqual(18, scoreWithFiveRemaining - scoreWithoutGoal);
        }

        private static BoardModel CreateBoardWithRedMove()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));

            return board;
        }
    }
}