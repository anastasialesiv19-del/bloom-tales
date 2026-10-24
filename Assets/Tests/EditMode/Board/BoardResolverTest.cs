using System.Linq;
using Core.Board;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Board
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
            int clearedCells = BoardResolver.ClearMatches(board,  matches).TotalClearedCells;
            
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

        [Test]
        public void ApplyGravity_WhenPieceFalls_ReturnsActualMovement()
        {
            var board = new BoardModel(4, 1);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 0, Piece.Empty());
            board.Set(2, 0, Piece.Empty());
            board.Set(3, 0, Piece.Normal(PieceColor.Blue));
            
            var pieceFalls = BoardResolver.ApplyGravity(board);
            Assert.AreEqual(1, pieceFalls.Count);
            Assert.AreEqual(new Pos(0,0), pieceFalls[0].From);
            Assert.AreEqual(new Pos(2,0), pieceFalls[0].To);
            Assert.AreEqual(Piece.Normal(PieceColor.Red), board.Get(pieceFalls[0].To.Row, pieceFalls[0].To.Col));
            Assert.AreEqual(Piece.Normal(PieceColor.Blue), board.Get(3, 0));
            Assert.IsTrue(board.Get(0, 0).IsEmpty);
            Assert.IsTrue(board.Get(1, 0).IsEmpty);
        }

        [Test]
        public void Refill_WhenBoardHasEmptyCells_ReturnsOnlyFilledPositions()
        {
            var board = new BoardModel(2, 2);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Empty());
            board.Set(1, 0, Piece.Empty());
            board.Set(1, 1, Piece.Empty());
            
            var pieceRefills = BoardResolver.Refill(board);
            
            Assert.AreEqual(3,  pieceRefills.Count);
            Assert.IsTrue(pieceRefills.Contains(new Pos(0,1)));
            Assert.IsTrue(pieceRefills.Contains(new Pos(1,0)));
            Assert.IsTrue(pieceRefills.Contains(new Pos(1,1)));
            Assert.AreEqual(PieceKind.Normal, board.Get(0,1).Kind);
            Assert.AreEqual(PieceKind.Normal, board.Get(1,0).Kind);
            Assert.AreEqual(PieceKind.Normal, board.Get(1,1).Kind);
            Assert.AreEqual( Piece.Normal(PieceColor.Red), board.Get(0,0));
        }
    }
}