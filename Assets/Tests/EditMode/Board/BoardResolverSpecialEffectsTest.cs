using Core.Contracts;
using Core.Board;
using Core.Matching;
using Core.Specials;
using NUnit.Framework;

namespace Tests.EditMode.Board
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

            var before = BoardSnapshotFactory.Capture(board);
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board,  matches, spawns);

            for (int row = 0; row < board.Height; row++)
            for (int col = 0; col < board.Width; col++)
            {
                if (row == 1 || col == 2)
                    Assert.IsTrue(board.Get(row, col).IsEmpty, $"({row}, {col}) must clear");
                else
                    Assert.AreEqual(before.Get(row, col), board.Get(row, col), $"({row}, {col}) must stay");
            }
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

            var before = BoardSnapshotFactory.Capture(board);
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board,  matches, spawns);

            for (int row = 0; row < board.Height; row++)
            for (int col = 0; col < board.Width; col++)
            {
                if (col >= 1)
                    Assert.IsTrue(board.Get(row, col).IsEmpty, $"({row}, {col}) must clear");
                else
                    Assert.AreEqual(before.Get(row, col), board.Get(row, col), $"({row}, {col}) must stay");
            }
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

            var before = BoardSnapshotFactory.Capture(board);
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board,  matches, spawns);

            for (int row = 0; row < board.Height; row++)
            for (int col = 0; col < board.Width; col++)
            {
                if (before.Get(row, col).Color == PieceColor.Green || (row == 2 && col == 2))
                    Assert.IsTrue(board.Get(row, col).IsEmpty, $"({row}, {col}) must clear");
                else
                    Assert.AreEqual(before.Get(row, col), board.Get(row, col), $"({row}, {col}) must stay");
            }
            Assert.IsTrue(board.Get(0,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,3).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,2).IsEmpty, board.ToDebugString());
        }

        [Test]
        public void ClearMatches_WithColorBombAndColoredSpecialCombo_ClearsExpandedSet()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.ColorBomb());
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.RowClear(PieceColor.Red));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var before = BoardSnapshotFactory.Capture(board);
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board,  matches, spawns);

            for (int row = 0; row < board.Height; row++)
            for (int col = 0; col < board.Width; col++)
            {
                if (!(row == 1 && col == 0))
                    Assert.IsTrue(board.Get(row, col).IsEmpty, $"({row}, {col}) must clear");
                else
                    Assert.AreEqual(before.Get(row, col), board.Get(row, col), $"({row}, {col}) must stay");
            }
            Assert.IsTrue(board.Get(0,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(0,3).IsEmpty, board.ToDebugString());
            Assert.IsFalse(board.Get(1,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,1).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,2).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(1,3).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2,0).IsEmpty, board.ToDebugString());
            Assert.IsTrue(board.Get(2, 2).IsEmpty, board.ToDebugString());
        }
    }
}
