using Core.Board;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class BoardResolverSpecialEffectsTest
    {
        [Test]
        public void ClearMatches_WithTriggeredRowClear_ClearsExpandedSet()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.RowClear(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board,  matches, spawns);

            Assert.IsTrue(board.Get(0,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,3).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,2).IsEmpty, board.ToDebugString());
        }

        [Test]
        public void ClearMatches_WithTriggeredAreaBomb_ClearsExpandedSet()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Red));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board,  matches, spawns);

            Assert.IsTrue(board.Get(0,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0,3).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,3).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,3).IsEmpty, board.ToDebugString());
        }

        [Test]
        public void ClearMatches_WithTriggeredColorBomb_ClearsExpandedSet()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Green));
            board.Set(0, 1, Piece.Normal(PieceColor.Red));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.ColumnClear(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board,  matches, spawns);
            
            Assert.IsTrue(board.Get(0,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,3).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,2).IsEmpty, board.ToDebugString());
        }
    }
}