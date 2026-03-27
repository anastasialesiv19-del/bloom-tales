using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintFinderAdaptiveTest
    {
         [Test]
        public void FindAdaptiveHint_ReturnsNoHint_WhenPolicyReturnsNone()
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

            var context = new HintContext(2, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNull(result.Explanation);
        }

        [Test]
        public void FindAdaptiveHint_ReturnsSoftHint_WhenBoardHasMoveAndIdleIsMedium()
        {
            var board = new BoardModel(2, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
            
            var context = new HintContext(5, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsTrue(result.Move.HasValue);
            Assert.AreEqual(HintStrength.Soft, result.Strength);
            
            bool matchCreated = MoveFinder.CreatesMatch(board, result.Move.Value.From, result.Move.Value.To);
            
            Assert.IsTrue(matchCreated);
        }

        [Test]
        public void FindAdaptiveHint_ReturnsStrongHint_WhenBoardHasMoveAndIdleIsLong()
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
            
            Assert.IsTrue(result.Move.HasValue);
            Assert.AreEqual(HintStrength.Strong, result.Strength);
            
            bool matchCreated = MoveFinder.CreatesMatch(board, result.Move.Value.From, result.Move.Value.To);
            
            Assert.IsTrue(matchCreated);
        }

        [Test]
        public void FindAdaptiveHint_WhenBestMoveScoreIsHigh_UpgradesSoftToStrong()
        {
            var board = new BoardModel(2, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            var context = new HintContext(5f, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNotNull(result.Move);
            Assert.IsNotNull(result.Explanation);
            Assert.GreaterOrEqual(result.Explanation.Score, 5);
            Assert.AreEqual(HintStrength.Strong, result.Strength);
        }
        
        [Test]
        public void FindAdaptiveHint_WhenBestMoveScoreIsLow_KeepsSoft()
        {
            var board = new BoardModel(2, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Green));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            var context = new HintContext(5f, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNotNull(result.Move);
            Assert.IsNotNull(result.Explanation);
            Assert.Less(result.Explanation.Score, 5);
            Assert.AreEqual(HintStrength.Soft, result.Strength);
        }
        
        [Test]
        public void FindAdaptiveHint_AppliesPolicyStrength_OnTopOfBestHint()
        {
            var board = new BoardModel(2, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Green));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            var context = new HintContext(5f, 0);
            var bestHint = HintFinder.FindBestHint(board);
            var adaptiveHint = HintFinder.FindAdaptiveHint(board, context);
            var expectedStrength = HintPolicy.GetStrength(context, bestHint.Explanation.Score);
            
            Assert.IsNotNull(bestHint.Move);
            Assert.IsNotNull(adaptiveHint.Move);
            Assert.IsNotNull(bestHint.Explanation);
            Assert.IsNotNull(adaptiveHint.Explanation);

            Assert.AreEqual(bestHint.Move.Value, adaptiveHint.Move.Value);
            Assert.AreEqual(bestHint.Explanation.Score, adaptiveHint.Explanation.Score);
            Assert.AreEqual(expectedStrength, adaptiveHint.Strength);
        }
    }
}