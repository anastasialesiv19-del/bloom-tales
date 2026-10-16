using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintObstacleScoringTest
    {
        [Test]
        public void HintScorer_WhenDirectRowClearWouldDamageObstacle_AddsObstacleBonus()
        {
            var board = new BoardModel(3, 4);
            Fill(board, PieceColor.Blue);

            board.Set(1, 1, Piece.RowClear(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 2, Piece.Empty());
            
            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var scoreWithoutObstacle = HintScorer.ScoreMove(board, move);
            
            board.SetObstacle(0, 2, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });
            
            var scoreWithObstacle = HintScorer.ScoreMove(board, move);
            var estimate = HintMoveOutcomeEstimator.Estimate(board, move);
            
            Assert.AreEqual(1, estimate.EstimatedObstacleDamage);
            Assert.AreEqual(4, scoreWithObstacle - scoreWithoutObstacle);
        }

        [Test]
        public void HintScorer_EstimateObstacleDamageCount_WhenMoveWouldDamageObstacle_ReturnsPositive()
        {
            var board = BoardWithMatchNextToObstacle();

            var move = new Move(new Pos(0, 1), new Pos(1, 1));

            var count = HintScorer.EstimateObstacleDamageCount(board, move);

            Assert.Greater(count, 0);
        }

        [Test]
        public void HintExplanation_WhenBestMoveDamagesObstacle_IncludesDamagesObstacleReason()
        {
            var board = BoardWithMatchNextToObstacle();

            var hint = HintFinder.FindBestHint(board);

            Assert.IsNotNull(hint.Explanation);
            Assert.IsTrue(hint.Explanation.Reasons.Contains(HintReasonType.DamagesObstacle));
        }

        [Test]
        public void HintScorer_WhenObstacleGoalIsActive_CapsBonusByRemainingObstacles()
        {
            var board = new BoardModel(3, 4);
            Fill(board, PieceColor.Blue);

            board.Set(1, 1, Piece.RowClear(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 1, Piece.Empty());
            board.Set(0, 2, Piece.Empty());
            
            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var scoreWithoutObstacles = HintScorer.ScoreMove(board, move);
            
            board.SetObstacle(0, 1, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            }); 
            board.SetObstacle(0, 2, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });
            
            var estimate = HintMoveOutcomeEstimator.Estimate(board, move);
            
            Assert.AreEqual(2, estimate.EstimatedObstacleDamage);
            
            var scoreWithObstaclesWithoutGoal = HintScorer.ScoreMove(board, move);

            var contextWithOneRemainingObstacle = new HintGoalContext(new Dictionary<PieceColor, int>(), 1);
            var contextWithTwoRemainingObstacles = new HintGoalContext(new Dictionary<PieceColor, int>(), 2);
            
            var scoreWithOneRemainingObstacle = HintScorer.ScoreMove(board, move, contextWithOneRemainingObstacle);
            var scoreWithTwoRemainingObstacles = HintScorer.ScoreMove(board, move, contextWithTwoRemainingObstacles);
            
            Assert.AreEqual(8, scoreWithObstaclesWithoutGoal - scoreWithoutObstacles);
            Assert.AreEqual(8, scoreWithOneRemainingObstacle - scoreWithoutObstacles);
            Assert.AreEqual(16, scoreWithTwoRemainingObstacles - scoreWithoutObstacles);
        }

        private static BoardModel BoardWithMatchNextToObstacle()
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

            board.SetObstacle(
                1,
                0,
                new ObstacleModel
                {
                    Kind = ObstacleKind.Box,
                    Health = 1
                });

            board.Set(1, 0, Piece.Empty());

            return board;
        }

        private static void Fill(BoardModel board, PieceColor color)
        {
            foreach (var pos in board.AllPositions())
                board.Set(pos.Row, pos.Col, Piece.Normal(color));
        }
    }
}