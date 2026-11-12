using Tests.EditMode.TestDoubles;
using System.Linq;
using Core.Board;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnCoreInvariantTest
    {
        [Test]
        public void TryProcessMove_InvalidSwap_DoesNotChangeBoard()
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

            var before = board.ToDebugString();
            var move = new Move(new Pos(1, 1), new Pos(2, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
            var after = board.ToDebugString();

            Assert.AreEqual(before, after);
            Assert.IsFalse(turnResult.WasApplied);
            Assert.AreEqual(FailureReason.InvalidSwap, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_NoMatch_DoesNotChangeBoard()
        {
            var board = new BoardModel(3, 4);
            
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var before = board.ToDebugString();
            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
            var after = board.ToDebugString();

            Assert.AreEqual(before, after);
            Assert.IsFalse(turnResult.WasApplied);
            Assert.AreEqual(FailureReason.NoMatchCreated, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_SuccessfulTurn_LeavesBoardWithoutEmptyCells()
        {
            var board = new BoardModel(3, 4);
            
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(turnResult.WasApplied);
            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
            Assert.IsFalse(board.AllPositions().Any(p => board.Get(p.Row, p.Col).IsEmpty));
        }

        [Test]
        public void TryProcessMove_TraceClearedCellsSum_MatchesTotalClearedCells()
        {
            var result = CascadeTestScenario.ResolveTwoWaves();
            
            Assert.IsTrue(result.WasApplied);
            Assert.AreEqual(2, result.CascadeCount);
            Assert.AreEqual(9, result.TotalClearedCells);
            Assert.AreEqual(9, result.Trace.Phases.Sum(phase => phase.ClearedCells));
        }
    }
}
