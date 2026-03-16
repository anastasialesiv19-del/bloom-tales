using Core.Board;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class SpecialMatchDetectorTest
    {
        [Test]
        public void Detect_ReturnsNoSpecialMatch_ForSimpleThreeInRow()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Yellow);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Yellow);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Red);
            board.Set(2, 3, PieceType.Red);
            
            var result = SpecialMatchDetector.Detect(board);
            
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Detect_ReturnsLine4_ForFourInRow()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Blue);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Red);
            board.Set(2, 3, PieceType.Red);
            
            var result = SpecialMatchDetector.Detect(board);
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].Type, SpecialMatchType.Line4);
            Assert.AreEqual(result[0].Size, 4);
        }

        [Test]
        public void Detect_ReturnsLine5_ForFiveInRow()
        {
            var board = new BoardModel(3, 5);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Blue);
            board.Set(0, 3, PieceType.Red);
            board.Set(0, 4, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Green);
            board.Set(1, 4, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Yellow);
            board.Set(2, 3, PieceType.Red);
            board.Set(2, 4, PieceType.Red);
            
            var result = SpecialMatchDetector.Detect(board);
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].Type, SpecialMatchType.Line5);
            Assert.AreEqual(result[0].Size, 5);
        }

        [Test]
        public void Detect_ReturnsTShape_ForTMatch()
        {
           
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Red);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);
            
            var result = SpecialMatchDetector.Detect(board);
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].Type, SpecialMatchType.TShape);
            Assert.AreEqual(result[0].Size, 5);
        }

        [Test]
        public void Detect_ReturnsLShape_ForLMatch()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Green);
            board.Set(0, 1, PieceType.Green);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Blue);
            board.Set(1, 1, PieceType.Yellow);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Blue);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);
            
            var result = SpecialMatchDetector.Detect(board);
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].Type, SpecialMatchType.LShape);
            Assert.AreEqual(result[0].Size, 5);
        }
        
    }
}  