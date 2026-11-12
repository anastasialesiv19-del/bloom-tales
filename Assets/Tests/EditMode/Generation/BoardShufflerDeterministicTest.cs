using System;
using Core.Board;
using Core.Generation;

using NUnit.Framework;
using Tests.EditMode.TestDoubles;

namespace Tests.EditMode.Generation
{
    public class BoardShufflerDeterministicTest
    {
        [Test]
        public void Shuffle_WhenUsingSequenceRandomProvider_ReordersBoardDeterministically()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Green));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));

            board.Set(2, 0, Piece.Normal(PieceColor.Blue));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));

            var provider = new SequenceRandomProvider(new[] {1, 1, 1, 1, 1, 1, 1, 1 });
            var before = board.ToDebugString();
            BoardShuffler.Shuffle(board, provider);
            var after = board.ToDebugString();
            
            var expected = new[,]
            {
                { PieceColor.Red, PieceColor.Blue, PieceColor.Green },
                { PieceColor.Blue, PieceColor.Red, PieceColor.Blue },
                { PieceColor.Red, PieceColor.Green, PieceColor.Green }
            };

            foreach (var pos in board.AllPositions())
            {
                Assert.AreEqual(Piece.Normal(expected[pos.Row, pos.Col]), board.Get(pos.Row, pos.Col));
            }
            Assert.AreNotEqual(before, after);
            Assert.IsFalse(BoardResolver.HasEmptyCells(board));
            Assert.IsTrue(BoardShuffler.IsPlayableBoard(board));
        }

        [Test]
        public void Shuffle_WhenBoardHasEmptyCells_StillThrows()
        {
            var board = new BoardModel(2, 3);

            board.Set(0, 0, Piece.Empty());
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Empty());

            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Empty());
            board.Set(1, 2, Piece.Normal(PieceColor.Green));

            var provider = new SequenceRandomProvider(new[] {2, 0, 3});
            
            Assert.Throws<InvalidOperationException>(() => BoardShuffler.Shuffle(board, provider));
        }
    }
}
