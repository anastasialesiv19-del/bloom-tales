using Tests.EditMode.TestDoubles;
using System;
using System.Collections.Generic;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;
using System.Linq;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorDirectSpecial_ColorBombTest
    {
        [Test]
        public void TryProcessMove_SwappedColorBombWithNormalPiece_ClearsAllCellsOfThatColor()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Green));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(2, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.IsTrue(result.TotalClearedCells > 0);
            Assert.GreaterOrEqual(result.TotalClearedCells, 5);

            var directPhase = result.Trace.Phases.Single(
                phase => phase.Type == TurnPhaseType.DirectSpecialActivation);

            var before = result.InitialBoardSnapshot;
            var after = directPhase.BoardAfterPhase;

            Assert.IsNotNull(after);

            for (int row = 0; row < board.Height; row++)
            {
                for (int col = 0; col < board.Width; col++)
                {
                    var original = before.Get(row, col);

                    bool shouldBeCleared =
                        original.Kind == PieceKind.ColorBomb ||
                        original.Color == PieceColor.Green;

                    if (shouldBeCleared)
                    {
                        Assert.IsTrue(
                            after.Get(row, col).IsEmpty,
                            $"Cell ({row}, {col}) should be cleared.");
                    }
                    else
                    {
                        Assert.AreEqual(
                            original,
                            after.Get(row, col),
                            $"Cell ({row}, {col}) should remain unchanged.");
                    }
                }
            }
        }

        [Test]
        public void TryProcessMove_SwappedColorBombWithNormalPiece_TriggersColoredSpecialsOfThatColor()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.ColumnClear(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Green));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.IsTrue(result.TotalClearedCells > 0);
            AssertDirectEffect(result, (row, col) => row == 2 || col == 1 || (row == 0 && col == 2) || (row == 1 && (col == 0 || col == 2 || col == 3)));
        }

        [Test]
        public void TryProcessMove_SwappedColorBombWithRowClear_TriggersThatColorOnBoard()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.RowClear(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var blueCount = 0;
            foreach (var pos in board.AllPositions())
                if (board.Get(pos.Row, pos.Col).Color == PieceColor.Blue)
                    blueCount++;

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => row == 1 || (row == 0 && (col == 1 || col == 3)) || (row == 2 && col == 1));
        }

        [Test]
        public void TryProcessMove_SwappedColorBombWithColumnClear_TriggersThatColorOnBoard()
        {
            var board = new BoardModel(4, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.ColumnClear(PieceColor.Yellow));
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Yellow));

            board.Set(3, 0, Piece.Normal(PieceColor.Green));
            board.Set(3, 1, Piece.Normal(PieceColor.Blue));
            board.Set(3, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var yellowCount = 0;
            foreach (var pos in board.AllPositions())
                if (board.Get(pos.Row, pos.Col).Color == PieceColor.Yellow)
                    yellowCount++;

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => col == 1 || (row == 0 && (col == 0 || col == 3)) || (row == 1 && col == 2) || (row == 2 && col == 3) || (row == 3 && col == 2));
        }

        [Test]
        public void TryProcessMove_SwappedColorBombWithAreaBomb_TriggersThatColorOnBoard()
        {
            var board = new BoardModel(4, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.AreaBomb(PieceColor.Yellow));
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Yellow));

            board.Set(3, 0, Piece.Normal(PieceColor.Green));
            board.Set(3, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(3, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var yellowCount = 0;
            foreach (var pos in board.AllPositions())
                if (board.Get(pos.Row, pos.Col).Color == PieceColor.Yellow)
                    yellowCount++;

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => (row <= 2 && col <= 2) || (row == 0 && col == 3) || (row == 2 && col == 3) || (row == 3 && (col == 1 || col == 2)));
        }

        [Test]
        public void TryProcessMove_SwappedColorBombWithColorBomb_ClearsWholeBoard()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.AreaBomb(PieceColor.Red));

            var allNonEmptyCells = new List<Pos>();
            foreach (var pos in board.AllPositions())
                if (!board.Get(pos.Row, pos.Col).IsEmpty)
                    allNonEmptyCells.Add(pos);
                    
            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
            var directPhase = result.Trace.Phases.Single(phase => phase.Type == TurnPhaseType.DirectSpecialActivation);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.AreEqual(allNonEmptyCells.Count, directPhase.ClearedCells);
            AssertDirectEffect(result, (row, col) => true);
            Assert.AreEqual(2, directPhase.SpecialActivations.Count);
            Assert.AreEqual(new Pos(1,1), directPhase.SpecialActivations[0].Position);
            Assert.AreEqual(new Pos(1,2), directPhase.SpecialActivations[1].Position);
            Assert.AreEqual(Piece.ColorBomb(), directPhase.SpecialActivations[0].Piece);
            Assert.AreEqual(Piece.ColorBomb(), directPhase.SpecialActivations[1].Piece);
            Assert.AreEqual(0, directPhase.SpecialActivations[0].ChainDepth);
            Assert.AreEqual(0, directPhase.SpecialActivations[1].ChainDepth);
            CollectionAssert.AreEquivalent(allNonEmptyCells, directPhase.SpecialActivations[0].AffectedCells);
            CollectionAssert.AreEquivalent(allNonEmptyCells, directPhase.SpecialActivations[1].AffectedCells);
        }

        [Test]
        public void TryProcessMove_SwappedColorBombWithColorBomb_RefillsBoardAfterClear()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.AreaBomb(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.RowClear(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            foreach (var pos in board.AllPositions())
                Assert.IsFalse(board.Get(pos.Row, pos.Col).IsEmpty);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
        }

        private static void AssertDirectEffect(TurnResult result, Func<int, int, bool> cleared)
        {
            var phase = result.Trace.Phases.Single(p => p.Type == TurnPhaseType.DirectSpecialActivation);
            var before = result.InitialBoardSnapshot;
            int expectedCount = 0;
            for (int row = 0; row < before.Height; row++)
            for (int col = 0; col < before.Width; col++)
            {
                if (cleared(row, col))
                {
                    expectedCount++;
                    Assert.IsTrue(phase.BoardAfterPhase.Get(row, col).IsEmpty, $"({row}, {col}) must clear");
                }
                else
                    Assert.AreEqual(before.Get(row, col), phase.BoardAfterPhase.Get(row, col), $"({row}, {col}) must stay");
            }
            Assert.AreEqual(expectedCount, phase.ClearedCells);
        }
    }
}
