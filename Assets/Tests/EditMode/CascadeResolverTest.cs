using Core.Board;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class CascadeResolverTest
    {
        [Test]
        public void Resolve_ReturnsNoCascade_WhenBoardHasNoMatches()
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

            var cascadeResult = CascadeResolver.Resolve(board);
            
            Assert.AreEqual(0, cascadeResult.CascadeCount, board.ToDebugString());
            Assert.AreEqual(0, cascadeResult.TotalClearedCells, board.ToDebugString());
            Assert.AreEqual(false, cascadeResult.HadAnyMatches, board.ToDebugString());
        }

        [Test]
        public void Resolve_ClearsAtLeastOneCascade_WhenBoardStartsWithMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Red);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);
            
            var cascadeResult = CascadeResolver.Resolve(board);
            
            Assert.IsTrue(cascadeResult.CascadeCount > 0, board.ToDebugString());
            Assert.IsTrue(cascadeResult.TotalClearedCells > 0, board.ToDebugString());
            Assert.AreEqual(true, cascadeResult.HadAnyMatches, board.ToDebugString());

        }

        [Test]
        public void Resolve_LeavesBoardWithoutEmptyCells()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Red);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);
            
            CascadeResolver.Resolve(board);
            bool itHasEmptyCells = BoardResolver.HasEmptyCells(board);
            
            Assert.IsFalse(itHasEmptyCells, board.ToDebugString());
        }

        [Test]
        public void Resolve_StopsWhenNoMatchesRemain()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Red);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);
            
            CascadeResolver.Resolve(board);
            
            Assert.AreEqual(0, MatchFinder.FindMatches(board).Count, board.ToDebugString());
        }
    }
}