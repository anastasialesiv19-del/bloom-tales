using System.Linq;
using Core.Board;
using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintMoveCollectorTest
    {
        [Test]
        public void Collect_DoesNotDuplicateMoves_WhenMoveMatchesMultipleRules()
        {
            var board = new BoardModel(3, 4);
            
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));

            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Green));

            var moves = HintMoveCollector.Collect(board);
            
            Assert.Greater(moves.Count, 0);
            
            var overlap = new Move(new Pos(1, 1), new Pos(1, 2));
            
            Assert.IsTrue(MoveFinder.CreatesMatch(board, overlap.From, overlap.To));
            Assert.IsTrue(HintMoveEvaluator.IsDirectSpecialActivation(board, overlap));
            Assert.AreEqual(1, moves.Count(move => move.Equals(overlap)));
            Assert.AreEqual(moves.Count, moves.Distinct().Count());
        }

    }
}
