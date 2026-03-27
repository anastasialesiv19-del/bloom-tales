using System.Linq;
using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintFinderExplanationTest
    {
         [Test]
        public void FindBestHint_NormalMove_ExplanationContainsCreatesMatch()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var context = new HintContext(9, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNotNull(result.Move);
            Assert.IsNotNull(result.Explanation);
            Assert.IsTrue(result.Explanation.Reasons.Contains(HintReasonType.CreatesMatch));
        }
        
        [Test]
        public void FindBestHint_SpecialCreatingMove_ExplanationContainsCreatesSpecial()
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

            var context = new HintContext(9, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNotNull(result.Explanation);
            Assert.IsTrue(result.Explanation.Reasons.Contains(HintReasonType.CreatesMatch));
            Assert.IsTrue(result.Explanation.Reasons.Contains(HintReasonType.CreatesSpecial));
        }
        
        [Test]
        public void FindBestHint_DirectSpecialMove_ExplanationContainsActivatesSpecialDirectly()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Green));

            var result = HintFinder.FindBestHint(board);
            
            Assert.IsNotNull(result.Explanation);
            Assert.IsTrue(result.Explanation.Reasons.Contains(HintReasonType.ActivatesSpecialDirectly));
            Assert.IsFalse(result.Explanation.Reasons.Contains(HintReasonType.CreatesMatch));
        }
        
        [Test]
        public void FindBestHint_ExplanationContainsSelectedMoveScore()
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

            var context = new HintContext(9, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            var expectedScore = 0;
            if (result.Move != null)
            {
                expectedScore = HintScorer.ScoreMove(board, result.Move.Value);
            }

            Assert.IsNotNull(result.Explanation);
            Assert.AreEqual(expectedScore, result.Explanation.Score);
        }
        
        [Test]
        public void FindBestHint_ReturnsExplanation_WithSelectedMoveScore()
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
            var expectedScore = HintScorer.ScoreMove(board, result.Move.Value);
            
            Assert.IsNotNull(result.Move);
            Assert.IsNotNull(result.Explanation);
            Assert.AreEqual(expectedScore, result.Explanation.Score);
            Assert.AreEqual(HintStrength.None, result.Strength);
        }
      
    }
}