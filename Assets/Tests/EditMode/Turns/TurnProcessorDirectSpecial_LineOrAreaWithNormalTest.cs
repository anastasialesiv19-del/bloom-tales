using System.Linq;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorDirectSpecial_LineOrAreaWithNormalTest
    {
        [Test]
        public void TryProcessMove_LineOrAreaWithNormal_DoesNotRequireColorMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.RowClear(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(0, 2));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.AreEqual(FailureReason.None, result.FailureReason);
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.SwapApplied));
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.DirectSpecialActivation));
        }
        [Test]
        public void RowClearWithNormal_VerticalSwap_ActivatesDestinationRow_AndPartnerStaysOutsideRow()
        {
            var board = new BoardModel(4, 4);
            Fill(board, PieceColor.Blue);

            board.Set(2, 1, Piece.RowClear(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));

            var move = new Move(new Pos(2, 1), new Pos(1, 1));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);

            AssertPhaseOrder(result, TurnPhaseType.SwapApplied, TurnPhaseType.DirectSpecialActivation);

            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.SwapApplied, out var swapPhase);
            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.DirectSpecialActivation, out var directPhase);

            Assert.AreEqual(PieceKind.RowClear, swapPhase.BoardAfterPhase.Get(1, 1).Kind);
            Assert.AreEqual(PieceKind.Normal, swapPhase.BoardAfterPhase.Get(2, 1).Kind);

            Assert.IsTrue(directPhase.BoardAfterPhase.Get(1, 1).IsEmpty);
            Assert.IsFalse(directPhase.BoardAfterPhase.Get(2, 1).IsEmpty);
        }

        [Test]
        public void RowClearWithNormal_HorizontalSwap_ActivatesDestinationRow_AndPartnerIsCleared()
        {
            var board = new BoardModel(4, 4);
            Fill(board, PieceColor.Blue);

            board.Set(1, 1, Piece.RowClear(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);

            AssertPhaseOrder(result, TurnPhaseType.SwapApplied, TurnPhaseType.DirectSpecialActivation);

            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.SwapApplied, out var swapPhase);
            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.DirectSpecialActivation, out var directPhase);

            Assert.AreEqual(PieceKind.Normal, swapPhase.BoardAfterPhase.Get(1, 1).Kind);
            Assert.AreEqual(PieceKind.RowClear, swapPhase.BoardAfterPhase.Get(1, 2).Kind);

            Assert.IsTrue(directPhase.BoardAfterPhase.Get(1, 1).IsEmpty);
            Assert.IsTrue(directPhase.BoardAfterPhase.Get(1, 2).IsEmpty);
        }

        [Test]
        public void ColumnClearWithNormal_HorizontalSwap_ActivatesDestinationColumn_AndPartnerStaysOutsideColumn()
        {
            var board = new BoardModel(4, 4);
            Fill(board, PieceColor.Blue);

            board.Set(1, 1, Piece.ColumnClear(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);

            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.SwapApplied, out var swapPhase);
            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.DirectSpecialActivation, out var directPhase);

            Assert.AreEqual(PieceKind.Normal, swapPhase.BoardAfterPhase.Get(1, 1).Kind);
            Assert.AreEqual(PieceKind.ColumnClear, swapPhase.BoardAfterPhase.Get(1, 2).Kind);

            Assert.IsFalse(directPhase.BoardAfterPhase.Get(1, 1).IsEmpty);
            Assert.IsTrue(directPhase.BoardAfterPhase.Get(1, 2).IsEmpty);
        }

        [Test]
        public void ColumnClearWithNormal_VerticalSwap_ActivatesDestinationColumn_AndPartnerIsCleared()
        {
            var board = new BoardModel(4, 4);
            Fill(board, PieceColor.Blue);

            board.Set(1, 1, Piece.ColumnClear(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));

            var move = new Move(new Pos(1, 1), new Pos(2, 1));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);

            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.SwapApplied, out var swapPhase);
            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.DirectSpecialActivation, out var directPhase);

            Assert.AreEqual(PieceKind.Normal, swapPhase.BoardAfterPhase.Get(1, 1).Kind);
            Assert.AreEqual(PieceKind.ColumnClear, swapPhase.BoardAfterPhase.Get(2, 1).Kind);

            Assert.IsTrue(directPhase.BoardAfterPhase.Get(1, 1).IsEmpty);
            Assert.IsTrue(directPhase.BoardAfterPhase.Get(2, 1).IsEmpty);
        }

        [Test]
        public void AreaBombWithNormal_DirectActivation_ActivatesFromDestination_AndClearsPartnerInsideArea()
        {
            var board = new BoardModel(4, 4);
            Fill(board, PieceColor.Blue);

            board.Set(1, 1, Piece.AreaBomb(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);

            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.SwapApplied, out var swapPhase);
            TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.DirectSpecialActivation, out var directPhase);

            Assert.AreEqual(PieceKind.Normal, swapPhase.BoardAfterPhase.Get(1, 1).Kind);
            Assert.AreEqual(PieceKind.AreaBomb, swapPhase.BoardAfterPhase.Get(1, 2).Kind);

            Assert.IsTrue(directPhase.BoardAfterPhase.Get(1, 1).IsEmpty);
            Assert.IsTrue(directPhase.BoardAfterPhase.Get(1, 2).IsEmpty);
        }

        private static void AssertPhaseOrder(
            TurnResult result,
            TurnPhaseType first,
            TurnPhaseType second)
        {
            var phases = result.Trace.Phases
                .Select(phase => phase.Type)
                .ToList();

            var firstIndex = phases.IndexOf(first);
            var secondIndex = phases.IndexOf(second);

            Assert.GreaterOrEqual(firstIndex, 0);
            Assert.GreaterOrEqual(secondIndex, 0);
            Assert.Less(firstIndex, secondIndex);
        }
        
        private static void Fill(BoardModel board, PieceColor color)
        {
            foreach (var pos in board.AllPositions())
                board.Set(pos.Row, pos.Col, Piece.Normal(color));
        }
    }
}