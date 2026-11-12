using System.Linq;
using Core.Board;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Board
{
    public class MoveFinderTest
    {
        [Test]
        public void AreAdjacent_ReturnsTrue_ForHorizontalNeighbors()
        {
            var a = new Pos(1, 1);
            var b = new Pos(1, 2);
            
            var isNeighbors = MoveValidator.AreAdjacent(a, b);
            
            Assert.IsTrue(isNeighbors);
        }

        [Test]
        public void AreAdjacent_ReturnsFalse_ForDiagonalCells()
        {
            var a = new Pos(1, 1);
            var b = new Pos(2, 2);
            
            var isNeighbors = MoveValidator.AreAdjacent(a, b);
            
            Assert.IsFalse(isNeighbors);
        }

        [Test]
        public void CreatesMatch_ReturnsTrue_WhenSwapCompletesThreeInRow()
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

            var a = new Pos(1, 2);
            var b = new Pos(1, 3);

            var before = board.ToDebugString();
            var isCreated = MoveFinder.CreatesMatch(board, a, b);
            var after = board.ToDebugString();

            Assert.IsTrue(isCreated);
            Assert.AreEqual(before, after, "CreatesMatch must rollback swap.\n" + after);
        }

        [Test]
        public void FindAllPossibleMoves_ReturnsAtLeastOneMove()
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

            var a = new Pos(1, 2);
            var b = new Pos(1, 3);

            var moves = MoveFinder.FindAllPossibleMoves(board);
            var containsExpected = moves.Any(m => (m.From.Equals(a) && m.To.Equals(b)) || (m.From.Equals(b) && m.To.Equals(a)));

            Assert.IsTrue(moves.Count >= 1);
            Assert.IsTrue(containsExpected, board.ToDebugString());
        }

        [Test]
        public void HasAnyPossibleMove_ReturnsFalse_ForDeadlockedBoard()
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


            var moves = MoveFinder.FindAllPossibleMoves(board);
            var deadlock = MoveFinder.IsDeadlocked(board);
            
            Assert.IsTrue(moves.Count == 0);
            Assert.IsTrue(deadlock, board.ToDebugString());
        }
    }
}