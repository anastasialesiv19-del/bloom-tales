using Core.Board;
using Core.Generation;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Generation
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

            var isPlayable = BoardShuffler.IsPlayableBoard(board);
            
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

            var isPlayable = BoardShuffler.IsPlayableBoard(board);
            var deadlock = MoveFinder.IsDeadlocked(board);
            
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
            var isPlayable = BoardShuffler.IsPlayableBoard(board);
            
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
                switch (type.Color)
                {
                    case PieceColor.Yellow:
                        yellowCount++;
                        break;
                    case PieceColor.Green:
                        greenCount++;
                        break;
                    case PieceColor.Red:
                        redCount++;
                        break;
                    case PieceColor.Blue:
                        blueCount++;
                        break;
                }
            }
            BoardShuffler.Shuffle(board);
            
            int newYellowCount = 0, newBlueCount = 0,  newGreenCount = 0 , newRedCount = 0;
            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                switch (type.Color)
                {
                    case PieceColor.Yellow:
                        newYellowCount++;
                        break;
                    case PieceColor.Green:
                        newGreenCount++;
                        break;
                    case PieceColor.Red:
                        newRedCount++;
                        break;
                    case PieceColor.Blue:
                        newBlueCount++;
                        break;
                }
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
                switch (type.Kind)
                {
                    case PieceKind.RowClear:
                        rowClear++;
                        break;
                    case PieceKind.ColumnClear:
                        columnClear++;
                        break;
                    case PieceKind.AreaBomb:
                        areaBomb++;
                        break;
                    case PieceKind.ColorBomb:
                        colorBomb++;
                        break;
                }
            }
            BoardShuffler.Shuffle(board);
            
            int newRowClear = 0, newColumnClear = 0,  newAreaBomb = 0 , newColorBomb = 0;
            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                switch (type.Kind)
                {
                    case PieceKind.RowClear:
                        newRowClear++;
                        break;
                    case PieceKind.ColumnClear:
                        newColumnClear++;
                        break;
                    case PieceKind.AreaBomb:
                        newAreaBomb++;
                        break;
                    case PieceKind.ColorBomb:
                        newColorBomb++;
                        break;
                }
            }
            
            Assert.AreEqual(colorBomb, newColorBomb);
            Assert.AreEqual(areaBomb, newAreaBomb);
            Assert.AreEqual(columnClear, newColumnClear);
            Assert.AreEqual(rowClear, newRowClear);
        }
    }
}