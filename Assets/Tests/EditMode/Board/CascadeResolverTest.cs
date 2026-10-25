using System.Collections.Generic;
using Core.Board;
using Core.Matching;
using NUnit.Framework;
using Tests.EditMode.TestDoubles;

namespace Tests.EditMode.Board
{
    public class CascadeResolverTest
    {
        [Test]
        public void Resolve_ReturnsNoCascade_WhenBoardHasNoMatches()
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

            var cascadeResult = CascadeResolver.Resolve(board);

            Assert.AreEqual(0, cascadeResult.CascadeCount, board.ToDebugString());
            Assert.AreEqual(0, cascadeResult.TotalClearedCells, board.ToDebugString());
            Assert.AreEqual(false, cascadeResult.HadAnyMatches, board.ToDebugString());
        }

        [Test]
        public void Resolve_ClearsAtLeastOneCascade_WhenBoardStartsWithMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Red));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var cascadeResult = CascadeResolver.Resolve(board);

            Assert.IsTrue(cascadeResult.CascadeCount > 0, board.ToDebugString());
            Assert.IsTrue(cascadeResult.TotalClearedCells > 0, board.ToDebugString());
            Assert.AreEqual(true, cascadeResult.HadAnyMatches, board.ToDebugString());
        }

        [Test]
        public void Resolve_LeavesBoardWithoutEmptyCells()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Red));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            CascadeResolver.Resolve(board);
            bool itHasEmptyCells = BoardResolver.HasEmptyCells(board);

            Assert.IsFalse(itHasEmptyCells, board.ToDebugString());
        }

        [Test]
        public void Resolve_StopsWhenNoMatchesRemain()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Red));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            CascadeResolver.Resolve(board);

            Assert.AreEqual(0, MatchFinder.FindMatches(board).Count, board.ToDebugString());
        }

        [Test]
        public void ResolveCascades_WhenMatchedSpecialIsTriggered_AppliesItsEffect()
        {
            var board = new BoardModel(3, 4);
            var colors = new[,]
            {
                { PieceColor.Yellow, PieceColor.Blue, PieceColor.Green, PieceColor.Red },
                { PieceColor.Green, PieceColor.Red, PieceColor.Green, PieceColor.Red },
                { PieceColor.Yellow, PieceColor.Blue, PieceColor.Green, PieceColor.Red }
            };
            foreach (var pos in board.AllPositions())
                board.Set(pos.Row, pos.Col, Piece.Normal(colors[pos.Row, pos.Col]));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Green));
            var provider = new SequenceRandomProvider(new[] { 0, 1, 2, 1, 2, 4, 2, 4, 0 });
            int callbacks = 0;
            var result = CascadeResolver.Resolve(board, provider, (index, cleared, snapshot, spawns, activations) =>
            {
                callbacks++;
                Assert.AreEqual(1, index);
                Assert.AreEqual(9, cleared);
                for (int row = 0; row < 3; row++)
                    Assert.AreEqual(Piece.Normal(colors[row, 0]), snapshot.Get(row, 0));
                var expected = new[,] { { 0, 1, 2 }, { 1, 2, 4 }, { 2, 4, 0 } };
                for (int row = 0; row < 3; row++)
                for (int col = 1; col < 4; col++)
                    Assert.AreEqual(Piece.Normal((PieceColor)expected[row, col - 1]), snapshot.Get(row, col));
                Assert.AreEqual(1, activations.Count);
                Assert.AreEqual(new Pos(1,2), activations[0].Position);
                Assert.AreEqual(Piece.AreaBomb(PieceColor.Green), activations[0].Piece);
                Assert.AreEqual(0, activations[0].ChainDepth);
                Assert.AreEqual(9, activations[0].AffectedCells.Count);
                CollectionAssert.AreEquivalent(new List<Pos>(){ 
                    new Pos(0,1),
                    new Pos(0,2),
                    new Pos(0,3),
                    new Pos(1,1),
                    new Pos(1,2),
                    new Pos(1,3),
                    new Pos(2,1),
                    new Pos(2,2),
                    new Pos(2,3),              
                }, activations[0].AffectedCells);
            });
            Assert.AreEqual(1, callbacks);
            Assert.AreEqual(1, result.CascadeCount);
            Assert.AreEqual(9, result.TotalClearedCells);
        }

        [TestCase(4, PieceKind.RowClear)]
        [TestCase(5, PieceKind.ColorBomb)]
        public void Resolve_WhenCascadeHasLongLine_CreatesSpecial(int lineLength, PieceKind expectedKind)
        {
            var board = new BoardModel(1, lineLength);

            for (int col = 0; col < lineLength; col++)
                board.Set(0, col, Piece.Normal(PieceColor.Green));

            var refillSequence = new int[lineLength - 1];

            for (int i = 0; i < refillSequence.Length; i++)
                refillSequence[i] = i % 2;

            var provider = new SequenceRandomProvider(refillSequence);

            var result = CascadeResolver.Resolve(board, provider);

            Assert.AreEqual(1, result.CascadeCount);
            Assert.AreEqual(expectedKind, board.Get(0, 0).Kind);

            for (int col = 1; col < lineLength; col++)
                Assert.AreEqual(PieceKind.Normal, board.Get(0, col).Kind);

            Assert.IsFalse(BoardResolver.HasEmptyCells(board));
            Assert.AreEqual(0, MatchFinder.FindMatches(board).Count);
        }
    }
}
