using System.Linq;
using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintFinderSelectionTest
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
            var directMoveToYellow = new Move(new Pos(1, 1), new Pos(1, 2));
            var normalMove = new Move(new Pos(0, 2), new Pos(0, 3));
            var directScoreYellow = HintScorer.ScoreMove(board, directMoveToYellow);
            var normalScore = HintScorer.ScoreMove(board, normalMove);
            
            var directMoveToBlue = new Move(new Pos(1, 1), new Pos(0, 1));
            var directScoreBlue = HintScorer.ScoreMove(board, directMoveToBlue);
            
            
            Assert.Greater(directScoreYellow, normalScore);
            Assert.Greater(directScoreYellow, directScoreBlue);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Move.HasValue);
            Assert.AreEqual(directMoveToYellow, result.Move.Value);
        }

        [Test]
        public void FindBestHint_ReturnsNeutralStrength_WhenBestMoveExists()
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
           
            Assert.IsNotNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNotNull(result.Explanation);
        }
    }
}