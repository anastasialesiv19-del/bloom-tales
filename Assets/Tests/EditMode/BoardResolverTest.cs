using System.Linq;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class BoardResolverTest
    {
        [Test]
        public void ClearMatches_SetsMatchedCellsToNone()
        {
            var board = new BoardModel(3, 4);
    
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            int clearedCells = BoardResolver.ClearMatches(board,  matches);
            
            Assert.AreEqual(5, clearedCells, board.ToDebugString());
            Assert.IsTrue(board.Get(0, 2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1, 0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1, 1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1, 2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2, 2).IsEmpty, board.ToDebugString());
            
            Assert.IsTrue(board.Get(0, 1).Color == PieceColor.Blue, board.ToDebugString());
            Assert.IsTrue(board.Get(1, 3).Color == PieceColor.Blue, board.ToDebugString());
            Assert.IsTrue(board.Get(2, 3).Color == PieceColor.Red, board.ToDebugString());
        }

        [Test]
        public void ApplyGravity_MovesPiecesDownWithinColumn()
        {
            var board = new BoardModel(3, 3);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Empty());
            board.Set(0, 2, Piece.Normal(PieceColor.Green));

            board.Set(1, 0, Piece.Empty());
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Empty());

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Empty());
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            
            BoardResolver.ApplyGravity(board);
            var expectedBoard = "_ _ _ \r\nY _ G \r\nY G G \r\n";
            Assert.AreEqual(expectedBoard, board.ToDebugString(), board.ToDebugString());
        }

        [Test]
        public void Refill_FillsAllEmptyCells()
        {
            var board = new BoardModel(3, 3);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Empty());
            board.Set(0, 2,Piece.Normal(PieceColor.Green));

            board.Set(1, 0, Piece.Empty());
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Empty());

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Empty());
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            
            BoardResolver.Refill(board);
            bool itHasEmpty =  BoardResolver.HasEmptyCells(board);
            
            Assert.IsFalse(itHasEmpty, board.ToDebugString());
            Assert.IsFalse(board.Get(0, 1).IsEmpty, board.ToDebugString());
            Assert.IsFalse(board.Get(1, 0).IsEmpty, board.ToDebugString());
            Assert.IsFalse(board.Get(1, 2).IsEmpty, board.ToDebugString());
            Assert.IsFalse(board.Get(2, 1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0, 0).Color == PieceColor.Yellow, board.ToDebugString());
            Assert.IsTrue(board.Get(1, 1).Color == PieceColor.Green, board.ToDebugString());
        }

        [Test]
        public void ClearGravityRefill_LeavesBoardWithoutEmptyCells()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            BoardResolver.ClearMatches(board,  matches);
            BoardResolver.ApplyGravity(board);
            BoardResolver.Refill(board);
            
            bool itHasEmpty =  BoardResolver.HasEmptyCells(board);
            
            Assert.IsFalse(itHasEmpty, board.ToDebugString());
        }
    }
}