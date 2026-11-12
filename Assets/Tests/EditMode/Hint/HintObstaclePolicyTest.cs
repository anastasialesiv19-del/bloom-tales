using Core.Board;
using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintObstaclePolicyTest
    {
        [Test]
        public void HintMoveCollector_WhenBoardHasObstacle_DoesNotThrow()
        {
            var board = CreateBoardWithValidMoveAndObstacle();

            Assert.DoesNotThrow(() => HintMoveCollector.Collect(board));
        }

        [Test]
        public void HintMoveCollector_WhenMoveTouchesObstacle_DoesNotIncludeObstacleMove()
        {
            var board = CreateBoardWithValidMoveAndObstacle();
            var moves = HintMoveCollector.Collect(board);

            Assert.Greater(moves.Count, 0);
            
            foreach (var move in moves)
            {
                Assert.IsFalse(board.HasObstacle(move.From.Row, move.From.Col));
                Assert.IsFalse(board.HasObstacle(move.To.Row, move.To.Col));
            }
        }

        [Test]
        public void HintFinder_WhenBoardHasObstacleAndValidMove_ReturnsHint()
        {
            var board = CreateBoardWithValidMoveAndObstacle();
            var hint = HintFinder.FindBestHint(board);

            Assert.IsNotNull(hint.Move);
        }

        [Test]
        public void HintMoveEvaluator_WhenMoveTouchesObstacle_ReturnsNone()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.RowClear(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.SetObstacle(0, 1, new ObstacleModel { Kind = ObstacleKind.Box, Health = 1 });

            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var type = HintMoveEvaluator.GetDirectSpecialHintType(board, move);

            Assert.AreEqual(DirectSpecialHintType.None, type);
        }

        [Test]
        public void HintScorer_WhenMoveTouchesObstacle_ReturnsZero()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.RowClear(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.SetObstacle(0, 1, new ObstacleModel { Kind = ObstacleKind.Box, Health = 1 });

            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var score = HintScorer.ScoreMove(board, move);

            Assert.AreEqual(0, score);
        }

        [Test]
        public void HintScorer_WhenMatchWouldDamageObstacle_AddsObstacleBonus()
        {
            var board = CreateBoardWithValidMoveAndObstacle();
            var move = new Move(new Pos(0, 1), new Pos(1, 1));
            var score = HintScorer.ScoreMove(board, move);

            Assert.Greater(score, 3);
        }

        private static BoardModel CreateBoardWithValidMoveAndObstacle()
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

            board.SetObstacle(1, 0, new ObstacleModel { Kind = ObstacleKind.Box, Health = 1 });
            board.Set(1, 0, Piece.Empty());

            return board;
        }
    }
}
