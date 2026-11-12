using System;
using System.Linq;
using Core.Board;
using Core.Contracts;
using Core.Turns;
using NUnit.Framework;
using Tests.EditMode.TestDoubles;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorPresentationContractTest
    {
        [Test]
        public void TryProcessMove_InvalidSwap_StoresEqualInitialAndFinalSnapshots()
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
            var move = new Move(new Pos(1, 1), new Pos(2, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            var initialSnapshot = turnResult.InitialBoardSnapshot;
            var finalSnapshot = turnResult.FinalBoardSnapshot;
            var after = board.ToDebugString();

            Assert.IsFalse(turnResult.WasSwapValid, board.ToDebugString());
            Assert.IsFalse(turnResult.WasApplied, board.ToDebugString());
            Assert.AreEqual(FailureReason.InvalidSwap, turnResult.FailureReason);
            Assert.IsNotNull(initialSnapshot);
            Assert.IsNotNull(finalSnapshot);
            Assert.IsTrue(AssertSnapshotsEqual(initialSnapshot, finalSnapshot), $"{initialSnapshot.ToDebugString()}/n {finalSnapshot.ToDebugString()}");
            Assert.AreEqual(before, after, $"{before}/n {after}");
        }

        [Test]
        public void TryProcessMove_LineOrAreaWithNormal_CapturesSwapBeforeDirectActivation()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.RowClear(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(turnResult.WasApplied, board.ToDebugString());
            Assert.IsFalse(turnResult.CreatedMatch, board.ToDebugString());
            
            var initialSnapshot = turnResult.InitialBoardSnapshot;
            var directSpecialSnapshot = turnResult.Trace.Phases.SingleOrDefault(p => p.Type == TurnPhaseType.DirectSpecialActivation).BoardAfterPhase;
            var finalSnapshot = turnResult.FinalBoardSnapshot;
            
            var phases = turnResult.Trace.Phases.ToList();
            var swapPhase = phases.Single(p => p.Type == TurnPhaseType.SwapApplied);
            var directPhase = phases.Single(p => p.Type == TurnPhaseType.DirectSpecialActivation);

            Assert.Less(phases.IndexOf(swapPhase), phases.IndexOf(directPhase), "Swap must happen before direct activation.");
            Assert.IsNotNull(swapPhase.BoardAfterPhase);
            Assert.AreEqual(Piece.RowClear(PieceColor.Red), swapPhase.BoardAfterPhase.Get(1, 1));
            Assert.AreEqual(Piece.Normal(PieceColor.Yellow), swapPhase.BoardAfterPhase.Get(1, 2));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.DirectSpecialActivation));
            
            Assert.IsNotNull(initialSnapshot);
            Assert.IsNotNull(directSpecialSnapshot);
            Assert.IsNotNull(finalSnapshot);
            Assert.IsTrue(AssertBoardEqualsSnapshot(board, turnResult.FinalBoardSnapshot), $"{board.ToDebugString()}/n {turnResult.FinalBoardSnapshot.ToDebugString()}");
        }

        [Test]
        public void TryProcessMove_NormalMatch_CapturesSnapshotsForVisibleMutatingPhases()
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

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            var initialSnapshot = turnResult.InitialBoardSnapshot;
            var finalSnapshot = turnResult.FinalBoardSnapshot;

            Assert.IsTrue(turnResult.WasApplied, board.ToDebugString());
            Assert.IsTrue(turnResult.CreatedMatch, board.ToDebugString());
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.SwapApplied));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.PrimaryMatchResolved));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.GravityApplied));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.RefillApplied));
            Assert.IsNotNull(turnResult.Trace.Phases.SingleOrDefault(p => p.Type == TurnPhaseType.SwapApplied).BoardAfterPhase);
            Assert.IsNotNull(turnResult.Trace.Phases.SingleOrDefault(p => p.Type == TurnPhaseType.PrimaryMatchResolved).BoardAfterPhase);
            Assert.IsNotNull(turnResult.Trace.Phases.SingleOrDefault(p => p.Type == TurnPhaseType.GravityApplied).BoardAfterPhase);
            Assert.IsNotNull(turnResult.Trace.Phases.SingleOrDefault(p => p.Type == TurnPhaseType.RefillApplied).BoardAfterPhase);
            Assert.IsNotNull(initialSnapshot);
            Assert.IsNotNull(finalSnapshot);
            Assert.IsTrue(AssertBoardEqualsSnapshot(board, finalSnapshot), $"{board.ToDebugString()}/n {turnResult.FinalBoardSnapshot.ToDebugString()}");
        }

        [Test]
        public void TryProcessMove_WhenShuffleOccurs_CapturesShuffleSnapshot()
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

            Assert.IsNotNull(turnResult.Trace.Phases.First(p => p.Type == TurnPhaseType.ShuffleApplied).BoardAfterPhase);
            Assert.AreEqual(turnResult.Trace.Phases.First(p => p.Type == TurnPhaseType.ShuffleApplied).BoardAfterPhase.ToDebugString(), turnResult.FinalBoardSnapshot.ToDebugString());
            Assert.AreEqual(turnResult.FinalBoardSnapshot.ToDebugString(), board.ToDebugString());
        }

        [Test]
        public void TryProcessMove_SnapshotsAreImmutableAfterProcessorReturns()
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

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            var mutatingSnapshot = turnResult.Trace.Phases.SingleOrDefault(p => p.Type == TurnPhaseType.PrimaryMatchResolved);
            var beforeInitialPiece = turnResult.InitialBoardSnapshot.Get(0, 1);
            var beforeFinalPiece = turnResult.FinalBoardSnapshot.Get(1, 3);
            var beforeMutatingPiece = mutatingSnapshot.BoardAfterPhase.Get(2, 2);

            board.Set(0, 1, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));

            var afterInitialPiece = turnResult.InitialBoardSnapshot.Get(0, 1);
            var afterFinalPiece = turnResult.FinalBoardSnapshot.Get(1, 3);
            var afterMutatingPiece = mutatingSnapshot.BoardAfterPhase.Get(2, 2);

            Assert.AreEqual(beforeInitialPiece, afterInitialPiece);
            Assert.AreEqual(beforeFinalPiece, afterFinalPiece);
            Assert.AreEqual(beforeMutatingPiece, afterMutatingPiece);
            Assert.AreEqual(board.Get(0,1).Color, PieceColor.Red);
            Assert.AreEqual(board.Get(1,3).Color, PieceColor.Yellow);
            Assert.AreEqual(board.Get(2,2).Color, PieceColor.Blue);
        }

        private bool AssertSnapshotsEqual(BoardSnapshot expectedSnapshot, BoardSnapshot actualSnapshot)
        {
            if (expectedSnapshot.Width != actualSnapshot.Width || expectedSnapshot.Height != actualSnapshot.Height)
            {
                throw new ArgumentException("Snapshot sizes do not match.");
            }

            for (var row = 0; row < expectedSnapshot.Height; row++)
            {
                for (var col = 0; col < expectedSnapshot.Width; col++)
                {
                    var expectedPiece = expectedSnapshot.Get(row, col);
                    var actualPiece = actualSnapshot.Get(row, col);
                    if (expectedPiece.Kind != actualPiece.Kind || expectedPiece.Color != actualPiece.Color)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool AssertBoardEqualsSnapshot(BoardModel board, BoardSnapshot snapshot)
        {
            if (board.Width != snapshot.Width || board.Height != snapshot.Height)
            {
                throw new ArgumentException("Snapshot sizes do not match.");
            }

            for (var row = 0; row < board.Height; row++)
            {
                for (var col = 0; col < board.Width; col++)
                {
                    var boardPiece = board.Get(row, col);
                    var snapshotPiece = snapshot.Get(row, col);
                    if (boardPiece.Kind != snapshotPiece.Kind || boardPiece.Color != snapshotPiece.Color)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}