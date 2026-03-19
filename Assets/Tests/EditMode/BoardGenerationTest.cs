using System.Linq;
using NUnit.Framework;
using Core.Generation;
using Core.Matching;


namespace Tests.EditMode
{
    public class BoardGenerationTest
    {
        [Test]
        public void GeneratedBoard_HasNoMatches()
        {
            var board = BoardGenerator.Generate(5, 4);
            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(0, matches.Count, board.ToDebugString());
        }
        
        [Test]
        public void GeneratedBoard_IsPlayable()
        {
            var board = BoardGenerator.Generate(5, 5);
            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            Assert.IsTrue(isPlayable,  board.ToDebugString());
        }
    }
}