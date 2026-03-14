using System.Linq;
using Core.Board;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class MoveFinderTest
    {
        [Test]
        public void AreAdjacent_ReturnsTrue_ForHorizontalNeighbors()
        {
            Pos a = new Pos(1, 1);
            Pos b = new Pos(1, 2);
            bool isNeighbors = MoveValidator.AreAdjacent(a, b);
            Assert.IsTrue(isNeighbors);
        }

        [Test]
        public void AreAdjacent_ReturnsFalse_ForDiagonalCells()
        {
            Pos a = new Pos(1, 1);
            Pos b = new Pos(2, 2);
            bool isNeighbors = MoveValidator.AreAdjacent(a, b);
            Assert.IsFalse(isNeighbors);
        }

        [Test]
        public void CreatesMatch_ReturnsTrue_WhenSwapCompletesThreeInRow()
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

            Pos a = new Pos(1, 2);
            Pos b = new Pos(1, 3);

            var before = board.ToDebugString();
            bool isCreated = MoveFinder.CreatesMatch(board, a, b);
            var after = board.ToDebugString();

            Assert.IsTrue(isCreated);
            Assert.AreEqual(before, after, "CreatesMatch must rollback swap.\n" + after);
        }

        [Test]
        public void FindAllPossibleMoves_ReturnsAtLeastOneMove()
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

            Pos a = new Pos(1, 2);
            Pos b = new Pos(1, 3);

            var moves = MoveFinder.FindAllPossibleMoves(board);

            bool containsExpected = moves.Any(m =>
                (m.From.Equals(a) && m.To.Equals(b)) ||
                (m.From.Equals(b) && m.To.Equals(a))
            );

            Assert.IsTrue(moves.Count >= 1);
            Assert.IsTrue(containsExpected, board.ToDebugString());
        }

        [Test]
        public void HasAnyPossibleMove_ReturnsFalse_ForDeadlockedBoard()
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


            var moves = MoveFinder.FindAllPossibleMoves(board);
            bool deadlock = MoveFinder.IsDeadlocked(board);
            Assert.IsTrue(moves.Count == 0);
            Assert.IsTrue(deadlock, board.ToDebugString());
        }
    }
}