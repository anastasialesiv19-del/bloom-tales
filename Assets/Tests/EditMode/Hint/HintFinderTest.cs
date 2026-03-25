using System.Linq;
using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintFinderTest
    {
        [Test]
        public void FindHint_ReturnsHint_WhenBoardHasPossibleMove()
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
            
            Assert.IsTrue(result.Move.HasValue);
        }

        [Test]
        public void FindHint_ReturnsBestScoredMove_WhenSeveralMovesExist()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var moveLarge = new Move(new Pos(1,2), new Pos(1,3));
            var result = HintFinder.FindBestHint(board);
            
            Assert.IsTrue(result.Move.HasValue);
            Assert.AreEqual(moveLarge.From, result.Move.Value.From);
            Assert.AreEqual(moveLarge.To, result.Move.Value.To);
        }

        [Test]
        public void FindHint_ReturnsNoHint_WhenBoardIsDeadlocked()
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
            
            Assert.IsFalse(result.Move.HasValue);
        }

        [Test]
        public void FindHint_ReturnsMoveThatCreatesMatch()
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

            Assert.IsTrue(result.Move.HasValue);
            
            bool matchCreated = MoveFinder.CreatesMatch(board, result.Move.Value.From, result.Move.Value.To);
            
            Assert.IsTrue(matchCreated);
        }

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
            
            Assert.IsFalse(result.Move.HasValue);
            Assert.AreEqual(HintStrength.None, result.Strength);
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
        public void FindBestHint_DirectSpecialMove_CanBeReturnedWithoutMatchCreation()
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
            var createsMatch = result.Move != null && MoveFinder.CreatesMatch(board, result.Move.Value.From, result.Move.Value.To);
            var isDirect = result.Move != null && HintMoveEvaluator.IsDirectSpecialActivation(board, result.Move.Value);
            
            Assert.IsNotNull(result.Move);
            Assert.IsFalse(createsMatch);
            Assert.IsTrue(isDirect);
            Assert.IsNotNull(result.Explanation);
            Assert.IsTrue(result.Explanation.Reasons.Contains(HintReasonType.ActivatesSpecialDirectly));
        }
        
        [Test]
        public void FindBestHint_WhenDirectSpecialAndNormalMatchExist_SelectsHigherScoreMove()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Green));

            var result = HintFinder.FindBestHint(board);
            var directMove = new Move(new Pos(0, 1), new Pos(1, 1));
            var normalMove = new Move(new Pos(0, 2), new Pos(0, 3));
            var directScore = HintScorer.ScoreMove(board, directMove);
            var normalScore = HintScorer.ScoreMove(board, normalMove);
            
            Assert.Greater(directScore, normalScore);
            if (result.Move != null) Assert.AreEqual(directMove, result.Move.Value);
        }
        
        [Test]
        public void Collect_DoesNotDuplicateMoves_WhenMoveMatchesMultipleRules()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Green));

            var moves = HintMoveCollector.Collect(board);
            Assert.AreEqual(moves.Count, moves.Distinct().Count());
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
    }
}