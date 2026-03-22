using Core.Board;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorTest
    {
        [Test]
        public void TryProcessMove_ReturnsNotApplied_WhenSwapIsOutOfBounds()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(0, 3), new Pos(0, 4));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            Assert.IsFalse(turnResult.WasSwapValid);
            Assert.IsFalse(turnResult.WasApplied);
            Assert.IsFalse(turnResult.CreatedMatch);
            Assert.AreEqual(0, turnResult.CascadeCount);
            Assert.AreEqual(0, turnResult.TotalClearedCells);
            Assert.AreEqual(FailureReason.InvalidSwap, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_ReturnsNotApplied_WhenSwapDoesNotCreateMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(turnResult.WasSwapValid, board.ToDebugString());
            Assert.IsFalse(turnResult.WasApplied, board.ToDebugString());
            Assert.IsFalse(turnResult.CreatedMatch, board.ToDebugString());
            Assert.AreEqual(0, turnResult.CascadeCount, board.ToDebugString());
            Assert.AreEqual(0, turnResult.TotalClearedCells, board.ToDebugString());
            Assert.AreEqual(FailureReason.NoMatchCreated, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_DoesNotChangeBoard_WhenSwapDoesNotCreateMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var before = board.ToDebugString();
            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            TurnProcessor.TryProcessMove(board, move);
            var after = board.ToDebugString();

            Assert.AreEqual(before, after);
        }

        [Test]
        public void TryProcessMove_AppliesSwapAndResolvesCascade_WhenSwapCreatesMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            bool itHasEmpty = BoardResolver.HasEmptyCells(board);

            Assert.IsFalse(itHasEmpty, board.ToDebugString());
            Assert.IsTrue(turnResult.WasSwapValid, board.ToDebugString());
            Assert.IsTrue(turnResult.WasApplied, board.ToDebugString());
            Assert.IsTrue(turnResult.CreatedMatch, board.ToDebugString());
            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_ReturnsCascadeInfo_WhenMoveSucceeds()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(turnResult.CascadeCount >= 0, board.ToDebugString());
            Assert.IsTrue(turnResult.TotalClearedCells > 0, board.ToDebugString());
            Assert.AreEqual(move, turnResult.Move);
            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
        }
    }
}