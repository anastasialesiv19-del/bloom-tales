using Core.Board;
using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class HintScorerTest
    {
        [Test]
        public void ScoreMove_ReturnsHigherScore_ForLargerMatch()
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
            int scoreLarge = HintScorer.ScoreMove(board, moveLarge);
            
            var moveSmall = new Move(new Pos(0,2), new Pos(0,3));
            int scoreSmall = HintScorer.ScoreMove(board, moveSmall);
            
            Assert.IsTrue(scoreLarge > scoreSmall, board.ToDebugString());
        }

        [Test]
        public void ScoreMove_ReturnsZero_WhenMoveCreatesNoMatch()
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
            
            var move = new Move(new Pos(1,2), new Pos(1,3));
            int score = HintScorer.ScoreMove(board, move);
            
            Assert.AreEqual(0, score, board.ToDebugString());
        }
    }
}