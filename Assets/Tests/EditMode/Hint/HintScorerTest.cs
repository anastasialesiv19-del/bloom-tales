using Core.Hint;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintScorerTest
    {
        [Test]
        public void ScoreMove_ReturnsHigherScore_ForLargerMatch()
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
            int scoreLarge = HintScorer.ScoreMove(board, moveLarge);
            
            var moveSmall = new Move(new Pos(0,2), new Pos(0,3));
            int scoreSmall = HintScorer.ScoreMove(board, moveSmall);
            
            Assert.IsTrue(scoreLarge > scoreSmall, board.ToDebugString());
        }

        [Test]
        public void ScoreMove_ReturnsZero_WhenMoveCreatesNoMatch()
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
            
            var move = new Move(new Pos(1,2), new Pos(1,3));
            int score = HintScorer.ScoreMove(board, move);
            
            Assert.AreEqual(0, score, board.ToDebugString());
        }
        
        [Test]
        public void ScoreMove_ColorBombWithColorBomb_IsHigherThanColorBombWithNormal()
        {
            var board1 = new BoardModel(3, 4);
            board1.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 2, Piece.Normal(PieceColor.Green));
            board1.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.ColorBomb());
            board1.Set(1, 2, Piece.ColorBomb());
            board1.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board1.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(2, 2, Piece.Normal(PieceColor.Green));
            board1.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var colorBombWithColorBomb = new Move(new Pos(1,1), new Pos(1,2));
            int colorBombWithColorBombScore = HintScorer.ScoreMove(board1, colorBombWithColorBomb);
            
            var board2 = new BoardModel(3, 4);
            board2.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 2, Piece.Normal(PieceColor.Green));
            board2.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board2.Set(1, 0, Piece.Normal(PieceColor.Green));
            board2.Set(1, 1, Piece.Normal(PieceColor.Green));
            board2.Set(1, 2, Piece.Normal(PieceColor.Red));
            board2.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board2.Set(2, 0, Piece.ColorBomb());
            board2.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(2, 2, Piece.Normal(PieceColor.Green));
            board2.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var colorBombWithNormal = new Move(new Pos(1,0), new Pos(2,0));
            int colorBombWithNormalScore = HintScorer.ScoreMove(board2, colorBombWithNormal);
            
            Assert.Greater(colorBombWithColorBombScore, colorBombWithNormalScore);
        }
        
        [Test]
         public void ScoreMove_LineWithArea_IsHigherThanLineOrAreaWithNormal()
        {
            var board1 = new BoardModel(3, 4);
            board1.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 2, Piece.Normal(PieceColor.Green));
            board1.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.RowClear(PieceColor.Yellow));
            board1.Set(1, 2, Piece.AreaBomb(PieceColor.Yellow));
            board1.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board1.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(2, 2, Piece.Normal(PieceColor.Green));
            board1.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var lineWithArea = new Move(new Pos(1,1), new Pos(1,2));
            int lineWithAreaScore = HintScorer.ScoreMove(board1, lineWithArea);
            
            var board2 = new BoardModel(3, 4);
            board2.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board2.Set(0, 2, Piece.Normal(PieceColor.Green));
            board2.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board2.Set(1, 0, Piece.Normal(PieceColor.Green));
            board2.Set(1, 1, Piece.Normal(PieceColor.Green));
            board2.Set(1, 2, Piece.Normal(PieceColor.Red));
            board2.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board2.Set(2, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(2, 2, Piece.Normal(PieceColor.Green));
            board2.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var lineWithNormal = new Move(new Pos(1,0), new Pos(2,0));
            int lineWithNormalScore = HintScorer.ScoreMove(board2, lineWithNormal);
            
            Assert.Greater(lineWithAreaScore, lineWithNormalScore);
        }

        [Test]
        public void FindBestHint_WhenMultipleDirectSpecialMovesExist_SelectsHigherPriorityCombo()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board.Set(0, 1, Piece.ColorBomb());
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.ColorBomb());
            board.Set(2, 1, Piece.ColorBomb());
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var result = HintFinder.FindBestHint(board);
            
            var lowerMove = new Move(new Pos(0,1), new Pos(0,2));
            int lowerScore = HintScorer.ScoreMove(board, lowerMove);
            
            var higherMove = new Move(new Pos(2,0), new Pos(2,1));
            int higherScore = HintScorer.ScoreMove(board, higherMove);
            
            Assert.Greater(higherScore, lowerScore);
            Assert.IsNotNull(result.Move);
            Assert.AreEqual(higherMove, result.Move.Value);
        }
        
        [Test]
         public void ScoreMove_ColorBombSpawn_IsHigherThanLineSpawn()
        {
            var board1 = new BoardModel(2, 4);
            board1.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 2, Piece.Normal(PieceColor.Green));
            board1.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Green));
            board1.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board1.Set(1, 3, Piece.Normal(PieceColor.Green));
            
            var line = new Move(new Pos(0,2), new Pos(1,2));
            int lineScore = HintScorer.ScoreMove(board1, line);
            
            var board2 = new BoardModel(2, 5);
            board2.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 2, Piece.Normal(PieceColor.Green));
            board2.Set(0, 3, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 4, Piece.Normal(PieceColor.Blue));
                            
            board2.Set(1, 0, Piece.Normal(PieceColor.Green));
            board2.Set(1, 1, Piece.Normal(PieceColor.Green));
            board2.Set(1, 2, Piece.Normal(PieceColor.Red));
            board2.Set(1, 3, Piece.Normal(PieceColor.Green));
            board2.Set(1, 4, Piece.Normal(PieceColor.Green));
            
            var colorBomb = new Move(new Pos(0,2), new Pos(1,2));
            int colorBombScore = HintScorer.ScoreMove(board2, colorBomb);
            
            Assert.Greater(colorBombScore, lineScore);
        }
        
         [Test]
        public void ScoreMove_AreaSpawn_IsHigherThanLineSpawn()
        {
            var board1 = new BoardModel(2, 4);
            board1.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board1.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board1.Set(0, 2, Piece.Normal(PieceColor.Green));
            board1.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Green));
            board1.Set(1, 2, Piece.Normal(PieceColor.Red));
            board1.Set(1, 3, Piece.Normal(PieceColor.Green));
            
            var line = new Move(new Pos(0,2), new Pos(1,2));
            int lineScore = HintScorer.ScoreMove(board1, line);
            
            var board2 = new BoardModel(3, 4);
            board2.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(0, 2, Piece.Normal(PieceColor.Green));
            board2.Set(0, 3, Piece.Normal(PieceColor.Blue));
                            
            board2.Set(1, 0, Piece.Normal(PieceColor.Green));
            board2.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board2.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board2.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board2.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board2.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board2.Set(2, 2, Piece.Normal(PieceColor.Blue));
            board2.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var area = new Move(new Pos(0,3), new Pos(0,2));
            int areaScore = HintScorer.ScoreMove(board2, area);
            
            Assert.Greater(areaScore, lineScore);
        }

        [Test]
        public void FindBestHint_PrefersHigherValueSpawn()
        {
            var board = new BoardModel(3, 5);
            board.Set(0, 0, Piece.Normal(PieceColor.Blue));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));
            board.Set(0, 4, Piece.Normal(PieceColor.Blue));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
            board.Set(1, 4, Piece.Normal(PieceColor.Red));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            board.Set(2, 4, Piece.Normal(PieceColor.Green));
            
            var result = HintFinder.FindBestHint(board);
            
            var lowerMove = new Move(new Pos(1,3), new Pos(2,3));
            int lowerScore = HintScorer.ScoreMove(board, lowerMove);
            
            var higherMove = new Move(new Pos(0,2), new Pos(1,2));
            int higherScore = HintScorer.ScoreMove(board, higherMove);
            
            Assert.Greater(higherScore, lowerScore);
            Assert.IsNotNull(result.Move);
            Assert.AreEqual(higherMove, result.Move.Value);
        }
    }
}