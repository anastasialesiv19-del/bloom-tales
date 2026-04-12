using System.Linq;
using Core.Generation;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;
using Tests.EditMode.TestDoubles;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorDeadlockTest
    {
        [Test]
        public void TryProcessMove_ShufflesBoard_WhenSuccessfulMoveLeavesDeadlock()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.ColorBomb());
            board.Set(0, 1, Piece.ColorBomb());
            board.Set(0, 2, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));

            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var provider = new SequenceRandomProvider(new[] { 0, 0, 1, 0, 0, 1, 1, 3, 3, 0, 0, 0, 0, 0, 0, 0, 1 });
            var turnResult = TurnProcessor.TryProcessMove(board, move, provider);

            Assert.IsTrue(turnResult.WasApplied, board.ToDebugString());
            Assert.IsTrue(turnResult.WasShuffled, board.ToDebugString());
        }

       [Test]
        public void TryProcessMove_DoesNotShuffle_WhenBoardRemainsPlayable()
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

            Assert.IsTrue(BoardShuffler.IsPlayableBoard(board));
            Assert.IsTrue(turnResult.WasSwapValid);
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsFalse(turnResult.WasShuffled);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
        }
    }
}