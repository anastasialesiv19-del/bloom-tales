using Core.Generation;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class BoardShufflerTest
    {
        [Test]
        public void IsPlayableBoard_ReturnsTrue_ForPlayableBoard()
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

            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            
            Assert.IsTrue(isPlayable);
        }

        [Test]
        public void IsPlayableBoard_ReturnsFalse_ForDeadlockedBoard()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));

            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
             bool deadlock = MoveFinder.IsDeadlocked(board);
            
            Assert.IsFalse(isPlayable);
            Assert.IsTrue(deadlock);
        }

        [Test]
        public void Shuffle_RemovesDeadlock()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));

            BoardShuffler.Shuffle(board);
            
            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            
            Assert.IsTrue(isPlayable);
        }

        [Test]
        public void Shuffle_LeavesBoardWithoutMatches()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));

            BoardShuffler.Shuffle(board);

            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void Shuffle_PreservesPieceCount()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                           
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));

            int yellowCount = 0, blueCount = 0,  greenCount = 0 , redCount = 0;

            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                if (type.Color == PieceColor.Yellow)
                    yellowCount++;
                else if (type.Color == PieceColor.Green)
                    greenCount++;
                else if (type.Color == PieceColor.Red)
                    redCount++;
                else if (type.Color == PieceColor.Blue)
                    blueCount++;
            }
            
            BoardShuffler.Shuffle(board);
            
            int newYellowCount = 0, newBlueCount = 0,  newGreenCount = 0 , newRedCount = 0;

            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                if (type.Color == PieceColor.Yellow)
                    newYellowCount++;
                else if (type.Color == PieceColor.Green)
                    newGreenCount++;
                else if (type.Color == PieceColor.Red)
                    newRedCount++;
                else if (type.Color == PieceColor.Blue)
                    newBlueCount++;
            }
            
            Assert.AreEqual(yellowCount, newYellowCount);
            Assert.AreEqual(blueCount, newBlueCount);
            Assert.AreEqual(redCount, newRedCount);
            Assert.AreEqual(greenCount, newGreenCount);
        }

        [Test]
        public void Shuffle_PreservesSpecialPiecesOnBoard()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.ColumnClear(PieceColor.Yellow));
            board.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.ColumnClear(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.AreaBomb(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));

            int rowClear = 0, columnClear = 0,  areaBomb = 0 , colorBomb = 0;

            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                if (type.Kind == PieceKind.RowClear)
                    rowClear++;
                else if (type.Kind == PieceKind.ColumnClear)
                    columnClear++;
                else if (type.Kind == PieceKind.AreaBomb)
                    areaBomb++;
                else if (type.Kind == PieceKind.ColorBomb)
                    colorBomb++;
            }
            
            BoardShuffler.Shuffle(board);
            
            int newRowClear = 0, newColumnClear = 0,  newAreaBomb = 0 , newColorBomb = 0;
            
            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                if (type.Kind == PieceKind.RowClear)
                    newRowClear++;
                else if (type.Kind == PieceKind.ColumnClear)
                    newColumnClear++;
                else if (type.Kind == PieceKind.AreaBomb)
                    newAreaBomb++;
                else if (type.Kind == PieceKind.ColorBomb)
                    newColorBomb++;
            }
            Assert.AreEqual(colorBomb, newColorBomb);
            Assert.AreEqual(areaBomb, newAreaBomb);
            Assert.AreEqual(columnClear, newColumnClear);
            Assert.AreEqual(rowClear, newRowClear);
        }
    }
}