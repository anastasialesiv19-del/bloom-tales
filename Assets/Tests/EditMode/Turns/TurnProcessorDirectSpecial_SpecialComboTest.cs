using Tests.EditMode.TestDoubles;
using System.Linq;
using System;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorDirectSpecial_SpecialComboTest
    {

        [Test]
        public void TryProcessMove_SwappedRowClearWithRowClear_ClearsBothRows()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.RowClear(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(2, 1));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => row == 1 || row == 2);
        }

        [Test]
        public void TryProcessMove_SwappedColumnClearWithColumnClear_ClearsBothColumns()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.ColumnClear(PieceColor.Green));
            board.Set(1, 2, Piece.ColumnClear(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => col == 1 || col == 2);
        }

        [Test]
        public void TryProcessMove_SwappedRowClearWithColumnClear_ClearsRowAndColumn()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.ColumnClear(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(2, 1));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => row == 2 || col == 1);
        }

        [Test]
        public void TryProcessMove_SwappedAreaBombWithRowClear_ClearsAreaAndRow()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.AreaBomb(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(2, 1));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => col <= 2 || row == 2);
        }

        [Test]
        public void TryProcessMove_SwappedAreaBombWithColumnClear_ClearsAreaAndColumn()
        {
            var board = new BoardModel(4, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.AreaBomb(PieceColor.Green));
            board.Set(1, 2, Piece.ColumnClear(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            board.Set(3, 0, Piece.Normal(PieceColor.Green));
            board.Set(3, 1, Piece.Normal(PieceColor.Blue));
            board.Set(3, 2, Piece.Normal(PieceColor.Green));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => (row <= 2 && col <= 2) || col == 2);
        }

        [Test]
        public void TryProcessMove_SwappedAreaBombWithAreaBomb_ClearsBothAreas()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.AreaBomb(PieceColor.Green));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            AssertDirectEffect(result, (row, col) => true);
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
