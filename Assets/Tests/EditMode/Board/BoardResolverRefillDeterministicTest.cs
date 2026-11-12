using Core.Board;
using NUnit.Framework;
using Tests.EditMode.TestDoubles;

namespace Tests.EditMode.Board
{
    public class BoardResolverRefillDeterministicTest
    {
        [Test]
        public void Refill_WhenUsingSequenceRandomProvider_FillsExpectedColorsInExpectedOrder()
        {
            var board = new BoardModel(2, 3);
            
            board.Set(0, 0, Piece.Empty());
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Empty());
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Empty());
            board.Set(1, 2, Piece.Normal(PieceColor.Green));

            var provider = new SequenceRandomProvider(new[] {1, 0, 2});
            BoardResolver.Refill(board, provider);
            
            Assert.AreEqual(PieceColor.Yellow, board.Get(0, 0).Color);
            Assert.AreEqual(PieceColor.Blue, board.Get(0, 1).Color);
            Assert.AreEqual(PieceColor.Red, board.Get(0, 2).Color);
            Assert.AreEqual(PieceColor.Red, board.Get(1, 0).Color);
            Assert.AreEqual(PieceColor.Green, board.Get(1, 1).Color);
            Assert.AreEqual(PieceColor.Green, board.Get(1, 2).Color);
        }
    }
}