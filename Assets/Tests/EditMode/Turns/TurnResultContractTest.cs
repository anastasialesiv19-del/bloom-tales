using Tests.EditMode.TestDoubles;
using System.Linq;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnResultContractTest
    {
        [Test]
        public void TryProcessMove_FailedTurn_ReturnsConsistentFailureResult()
        {
            var board1 = new BoardModel(3, 4);

            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 2, Piece.Normal(PieceColor.Green));
            board1.Set(0, 3, Piece.Normal(PieceColor.Red));

            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Green));
            board1.Set(1, 2, Piece.Normal(PieceColor.Red));
            board1.Set(1, 3, Piece.Normal(PieceColor.Yellow));

            board1.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(2, 2, Piece.Normal(PieceColor.Green));
            board1.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(0, 0), new Pos(0, 2));
            var turnResult = TurnProcessor.TryProcessMove(board1, move, new SeededRandomProvider());

            Assert.AreNotEqual(FailureReason.None, turnResult.FailureReason);
            Assert.IsFalse(turnResult.WasApplied);
            Assert.AreEqual(0, turnResult.CascadeCount);
            Assert.AreEqual(0, turnResult.TotalClearedCells);
            Assert.AreEqual(FailureReason.InvalidSwap, turnResult.FailureReason);
            Assert.IsFalse(turnResult.WasShuffled);
            Assert.IsFalse(turnResult.CreatedMatch);
            Assert.AreEqual(1, turnResult.Trace.Phases.Count);
            Assert.AreEqual(TurnPhaseType.InvalidSwap, turnResult.Trace.Phases[0].Type);

            var board2 = new BoardModel(3, 4);
            board2.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board2.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 2, Piece.Normal(PieceColor.Green));
            board2.Set(0, 3, Piece.Normal(PieceColor.Red));

            board2.Set(1, 0, Piece.Normal(PieceColor.Green));
            board2.Set(1, 1, Piece.Normal(PieceColor.Green));
            board2.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board2.Set(1, 3, Piece.Normal(PieceColor.Yellow));

            board2.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board2.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(2, 2, Piece.Normal(PieceColor.Green));
            board2.Set(2, 3, Piece.Normal(PieceColor.Red));

            move = new Move(new Pos(1,2), new Pos(1,3));
            turnResult = TurnProcessor.TryProcessMove(board2, move);

            Assert.AreNotEqual(FailureReason.None, turnResult.FailureReason);
            Assert.IsFalse(turnResult.WasApplied);
            Assert.AreEqual(0, turnResult.CascadeCount);
            Assert.AreEqual(0, turnResult.TotalClearedCells);
            Assert.AreEqual(FailureReason.NoMatchCreated, turnResult.FailureReason);
            Assert.IsFalse(turnResult.WasShuffled);
            Assert.IsFalse(turnResult.CreatedMatch);
            Assert.AreEqual(1, turnResult.Trace.Phases.Count);
            Assert.AreEqual(TurnPhaseType.NoMatchCreated, turnResult.Trace.Phases[0].Type);
        }

        [Test]
        public void TryProcessMove_SuccessfulTurn_ReturnsConsistentSuccessResult()
        {
            var board1 = new BoardModel(3, 4);

            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 2, Piece.Normal(PieceColor.Green));
            board1.Set(0, 3, Piece.Normal(PieceColor.Red));

            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Green));
            board1.Set(1, 2, Piece.Normal(PieceColor.Red));
            board1.Set(1, 3, Piece.Normal(PieceColor.Yellow));

            board1.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(2, 2, Piece.Normal(PieceColor.Green));
            board1.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1,2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board1, move, new SeededRandomProvider());

            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.WasSwapValid);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.Greater(turnResult.TotalClearedCells, 0);
            Assert.AreEqual(turnResult.TotalClearedCells, turnResult.ClearStats.TotalClearedCells);
        }

        [Test]
        public void TryProcessMove_SuccessfulTurn_TraceEndsWithTurnCompleted()
        {
            var board1 = new BoardModel(3, 4);

            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 2, Piece.Normal(PieceColor.Green));
            board1.Set(0, 3, Piece.Normal(PieceColor.Red));

            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Green));
            board1.Set(1, 2, Piece.Normal(PieceColor.Red));
            board1.Set(1, 3, Piece.Normal(PieceColor.Green));

            board1.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(2, 2, Piece.Normal(PieceColor.Green));
            board1.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1,2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board1, move, new SeededRandomProvider());

            Assert.IsTrue(turnResult.WasApplied);
            Assert.AreEqual(TurnPhaseType.TurnCompleted, turnResult.Trace.Phases[^1].Type);
            Assert.IsFalse(turnResult.Trace.Phases.Count == 0);
        }

        [Test]
        public void TryProcessMove_CascadeTrace_UsesSequentialCascadeIndexes()
        {
            var result = CascadeTestScenario.ResolveTwoWaves();
            Assert.IsTrue(result.WasApplied);
            Assert.AreEqual(2, result.CascadeCount);
            var phases = TurnTraceQueries.GetCascadePhases(result.Trace);
            Assert.AreEqual(2, phases.Count);
            Assert.AreEqual(1, phases[0].CascadeIndex);
            Assert.AreEqual(2, phases[1].CascadeIndex);
            Assert.IsFalse(result.WasShuffled);
            Assert.IsFalse(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.ShuffleApplied));
        }
    }
}
