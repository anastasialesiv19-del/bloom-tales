using System.Linq;
using Core.Board;
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
        public void MatchFinder_FindsHorizontalMatch()
        {
            var board = BoardGenerator.Generate(3, 2);
            board.Set(0, 0, PieceType.Green);
            board.Set(1, 0, PieceType.Green);
            board.Set(2, 0, PieceType.Green);
            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(1, matches.Count, board.ToDebugString());
            var cells = matches[0].Cells;
            Assert.AreEqual(3, cells.Count, board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 0 && p.Col == 0), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 0), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 2 && p.Col == 0), board.ToDebugString());
        }

        [Test]
        public void MatchFinder_FindsVerticalMatch()
        {
            var board = BoardGenerator.Generate(2, 3);
            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(1, matches.Count, board.ToDebugString());
            var cells = matches[0].Cells;
            Assert.AreEqual(3, cells.Count);
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 0), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 1), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 2), board.ToDebugString());
        }

        [Test]
        public void MergeOverlaps_MergesCrossIntoSingleMatch()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Blue);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Green);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);

            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(1, matches.Count, board.ToDebugString());
            var cells = matches[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.IsTrue(cells.Any(p => p.Row == 0 && p.Col == 2), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 1), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 2), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 3), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 2 && p.Col == 2), board.ToDebugString());

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