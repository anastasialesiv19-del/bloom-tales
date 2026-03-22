using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class TurnProcessorDirectSpecialActivationTest
    {
        [Test]
        public void TryProcessMove_SwappedRowClear_ActivatesWholeRow()
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

            var before1 = board.Get(2, 1);
            var before2 = board.Get(1, 3);
            var before3 = board.Get(0, 0);
            var move = new Move(new Pos(1, 2), new Pos(2, 2));
            var result = TurnProcessor.TryProcessMove(board, move);
            var after1 = board.Get(2, 1);
            var after2 = board.Get(1, 3);
            var after3 = board.Get(0, 0);

            Assert.IsTrue(result.TotalClearedCells > 0);
            Assert.GreaterOrEqual(result.TotalClearedCells, board.Width);
        }

        [Test]
        public void TryProcessMove_SwappedColumnClear_ActivatesWholeColumn()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.ColumnClear(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.TotalClearedCells > 0);
            Assert.GreaterOrEqual(result.TotalClearedCells, board.Height);
        }

        [Test]
        public void TryProcessMove_SwappedAreaBomb_ActivatesArea()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(2, 2));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.TotalClearedCells > 0);
            Assert.GreaterOrEqual(result.TotalClearedCells, 9);
        }

        [Test]
        public void TryProcessMove_SwappedSpecial_DoesNotRequireColorMatch()
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
            Assert.IsTrue(result.TotalClearedCells > 0);
            Assert.IsTrue(result.TotalClearedCells > board.Width);
            Assert.IsTrue(result.FailureReason == FailureReason.None);
        }

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
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.IsTrue(result.TotalClearedCells > 0);
            Assert.GreaterOrEqual(result.TotalClearedCells, 5);
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
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.IsTrue(result.TotalClearedCells > 0);
            Assert.GreaterOrEqual(result.TotalClearedCells, 10);
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
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.GreaterOrEqual(result.TotalClearedCells, blueCount);
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
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.GreaterOrEqual(result.TotalClearedCells, yellowCount);
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
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.GreaterOrEqual(result.TotalClearedCells, yellowCount);
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

            var allNonEmptyCells = 0;
            foreach (var pos in board.AllPositions())
                if (!board.Get(pos.Row, pos.Col).IsEmpty)
                    allNonEmptyCells++;

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.AreEqual(allNonEmptyCells, result.TotalClearedCells);
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
            var result = TurnProcessor.TryProcessMove(board, move);
            
            foreach (var pos in board.AllPositions())
                Assert.IsFalse(board.Get(pos.Row, pos.Col).IsEmpty);
            
            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
        }
    }
}