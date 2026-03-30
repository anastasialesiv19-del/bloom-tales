using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintSimulationSafetyTest
    {
        [Test]
        public void FindBestHint_SpecialEvaluation_DoesNotChangeBoard()
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

            var before = board.ToDebugString();
            HintFinder.FindBestHint(board);
            var after = board.ToDebugString();
            
            Assert.That(before, Is.EqualTo(after));
        }

        [Test]
        public void ScoreMove_MatchCreatingMove_DoesNotChangeBoard()
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
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));
            board.Set(2, 3, Piece.Normal(PieceColor.Yellow));

            var before = board.ToDebugString();
            HintScorer.ScoreMove(board, new Move(new Pos(1,2), new Pos(1,3)));
            var after = board.ToDebugString();
            
            Assert.That(before, Is.EqualTo(after));
        }

        [Test]
        public void ScoreMove_SpecialCreatingMove_DoesNotChangeBoard()
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

            var before = board.ToDebugString();
            HintScorer.ScoreMove(board, new Move(new Pos(1,2), new Pos(1,3)));
            var after = board.ToDebugString();
            
            Assert.That(before, Is.EqualTo(after));
        }
    }
}