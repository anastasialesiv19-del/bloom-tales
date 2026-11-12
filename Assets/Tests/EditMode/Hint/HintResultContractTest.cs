using Core.Board;
using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintResultContractTest
    {
        [Test]
        public void FindBestHint_NoMove_ReturnsConsistentEmptyResult()
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
            
            var result = HintFinder.FindBestHint(board);
            
            Assert.IsNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNull(result.Explanation);
        }

        [Test]
        public void FindAdaptiveHint_NoMove_ReturnsConsistentEmptyResult()
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
            
            var context = new HintContext(4, 0); 
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNull(result.Explanation);
        }

        [Test]
        public void FindBestHint_MoveExists_ReturnsNeutralStrengthWithExplanation()
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
            
            var result = HintFinder.FindBestHint(board);
            
            Assert.IsNotNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNotNull(result.Explanation);
            Assert.Greater(result.Explanation.Score, 0);
        }

        [Test]
        public void FindAdaptiveHint_MoveExists_ReturnsStrengthWithExplanation()
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
            
            var context = new HintContext(5, 0); 
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNotNull(result.Move);
            Assert.AreNotEqual(HintStrength.None, result.Strength);
            Assert.IsNotNull(result.Explanation);
            Assert.Greater(result.Explanation.Score, 0);
        }
    }
}