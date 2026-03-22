using System.Linq;
using Core.Generation;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Matching
{
    public class MatchFinderTest
    {
           [Test]
        public void MatchFinder_FindsHorizontalMatch()
        {
            var board = BoardGenerator.Generate(3, 2);
            board.Set(0, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
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
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
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
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

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
        public void FindMatches_MatchesNormalPiecesByColor()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(1, matches.Count, board.ToDebugString());
            var cells = matches[0].Cells;
            Assert.AreEqual(3, cells.Count);
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 1), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 2), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 3), board.ToDebugString());
        }

        [Test]
        public void FindMatches_MatchesSpecialAndNormalPiecesOfSameColor()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.RowClear(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(1, matches.Count, board.ToDebugString());
            var cells = matches[0].Cells;
            Assert.AreEqual(3, cells.Count);
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 1), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 2), board.ToDebugString());
            Assert.IsTrue(cells.Any(p => p.Row == 1 && p.Col == 3), board.ToDebugString());
        }

        [Test]
        public void FindMatches_DoesNotMatchEmpty()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Empty());

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(0, matches.Count, board.ToDebugString());
        }

        [Test]
        public void FindMatches_DoesNotTreatColorBombAsColoredPiece()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.ColorBomb());
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.ColorBomb());

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            Assert.AreEqual(0, matches.Count, board.ToDebugString());
        }
    }
}