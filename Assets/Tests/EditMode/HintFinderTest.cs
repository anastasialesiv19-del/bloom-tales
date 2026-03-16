using Core.Board;
using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class HintFinderTest
    {
        [Test]
        public void FindHint_ReturnsHint_WhenBoardHasPossibleMove()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);

            var result = HintFinder.FindBestHint(board);
            
            Assert.IsTrue(result.Move.HasValue);
        }

        [Test]
        public void FindHint_ReturnsBestScoredMove_WhenSeveralMovesExist()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Blue);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Blue);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);
            
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
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Red);
            board.Set(1, 1, PieceType.Yellow);
            board.Set(1, 2, PieceType.Blue);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Green);
            board.Set(2, 1, PieceType.Red);
            board.Set(2, 2, PieceType.Yellow);
            board.Set(2, 3, PieceType.Blue);
            
            var result = HintFinder.FindBestHint(board);
            
            Assert.IsFalse(result.Move.HasValue);
        }

        [Test]
        public void FindHint_ReturnsMoveThatCreatesMatch()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);

            var result = HintFinder.FindBestHint(board);

            Assert.IsTrue(result.Move.HasValue);
            
            bool matchCreated = MoveFinder.CreatesMatch(board, result.Move.Value.From, result.Move.Value.To);
            
            Assert.IsTrue(matchCreated);
        }

        [Test]
        public void FindAdaptiveHint_ReturnsNoHint_WhenPolicyReturnsNone()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);

            var context = new HintContext(2, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsFalse(result.Move.HasValue);
            Assert.AreEqual(HintStrength.None, result.Strength);
        }

        [Test]
        public void FindAdaptiveHint_ReturnsSoftHint_WhenBoardHasMoveAndIdleIsMedium()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);

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
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);

            var context = new HintContext(9, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsTrue(result.Move.HasValue);
            Assert.AreEqual(HintStrength.Strong, result.Strength);
            
            bool matchCreated = MoveFinder.CreatesMatch(board, result.Move.Value.From, result.Move.Value.To);
            
            Assert.IsTrue(matchCreated);
        }
    }
}