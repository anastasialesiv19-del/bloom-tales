using Core.Board;
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
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);

            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            
            Assert.IsTrue(isPlayable);
        }

        [Test]
        public void IsPlayableBoard_ReturnsFalse_ForDeadlockedBoard()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Red);
            board.Set(1, 1, PieceType.Yellow);
            board.Set(1, 2, PieceType.Blue);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Green);
            board.Set(2, 1, PieceType.Red);
            board.Set(2, 2, PieceType.Yellow);
            board.Set(2, 3, PieceType.Blue);

            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
             bool deadlock = MoveFinder.IsDeadlocked(board);
            
            Assert.IsFalse(isPlayable);
            Assert.IsTrue(deadlock);
        }

        [Test]
        public void Shuffle_RemovesDeadlock()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Red);
            board.Set(1, 1, PieceType.Yellow);
            board.Set(1, 2, PieceType.Blue);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Green);
            board.Set(2, 1, PieceType.Red);
            board.Set(2, 2, PieceType.Yellow);
            board.Set(2, 3, PieceType.Blue);

            BoardShuffler.Shuffle(board);
            
            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            
            Assert.IsTrue(isPlayable);
        }

        [Test]
        public void Shuffle_LeavesBoardWithoutMatches()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Red);
            board.Set(1, 1, PieceType.Yellow);
            board.Set(1, 2, PieceType.Blue);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Green);
            board.Set(2, 1, PieceType.Red);
            board.Set(2, 2, PieceType.Yellow);
            board.Set(2, 3, PieceType.Blue);

            BoardShuffler.Shuffle(board);

            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void Shuffle_PreservesPieceCount()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Red);
            board.Set(1, 1, PieceType.Yellow);
            board.Set(1, 2, PieceType.Blue);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Green);
            board.Set(2, 1, PieceType.Red);
            board.Set(2, 2, PieceType.Yellow);
            board.Set(2, 3, PieceType.Blue);

            int yellowCount = 0, blueCount = 0,  greenCount = 0 , redCount = 0;

            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                if (type == PieceType.Yellow)
                    yellowCount++;
                else if (type == PieceType.Green)
                    greenCount++;
                else if (type == PieceType.Red)
                    redCount++;
                else if (type == PieceType.Blue)
                    blueCount++;
            }
            
            BoardShuffler.Shuffle(board);
            
            int newYellowCount = 0, newBlueCount = 0,  newGreenCount = 0 , newRedCount = 0;

            foreach (var pos in board.AllPositions())
            {
                var type = board.Get(pos.Row, pos.Col);
                if (type == PieceType.Yellow)
                    newYellowCount++;
                else if (type == PieceType.Green)
                    newGreenCount++;
                else if (type == PieceType.Red)
                    newRedCount++;
                else if (type == PieceType.Blue)
                    newBlueCount++;
            }
            
            Assert.AreEqual(yellowCount, newYellowCount);
            Assert.AreEqual(blueCount, newBlueCount);
            Assert.AreEqual(redCount, newRedCount);
            Assert.AreEqual(greenCount, newGreenCount);
        }
    }
}