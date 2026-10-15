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
            var result = TurnProcessor.TryProcessMove(board, move);

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
            var directPhase = result.Trace.Phases.Single(phase => phase.Type == TurnPhaseType.DirectSpecialActivation);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.AreEqual(allNonEmptyCells, directPhase.ClearedCells);
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