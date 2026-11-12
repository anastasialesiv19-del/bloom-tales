using System;
using Core.Board;
using Core.Contracts;
using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintPresentationContractTest
    {
        [Test]
        public void FindBestHint_ReturnsSnapshotMatchingInputBoard()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var before = board.ToDebugString();
            var result = HintFinder.FindBestHint(board);
            var after = board.ToDebugString();
            
            Assert.IsNotNull(result.BoardSnapshot);
            Assert.IsTrue(AssertBoardEqualsSnapshot(board, result.BoardSnapshot), $"{board.ToDebugString()} != {result.BoardSnapshot.ToDebugString()}");
            Assert.AreEqual(before, after);
        }

        [Test]
        public void FindAdaptiveHint_ReturnsSnapshotMatchingInputBoard()
        {
            var board = new BoardModel(2, 4);
            
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.ColorBomb());
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                    
            var before = board.ToDebugString();
            var context = new HintContext(8, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            var after = board.ToDebugString();

            Assert.IsNotNull(result.BoardSnapshot);
            Assert.IsTrue(AssertBoardEqualsSnapshot(board, result.BoardSnapshot), $"{board.ToDebugString()} != {result.BoardSnapshot.ToDebugString()}");
            Assert.AreEqual(before, after);
        }

        [Test]
        public void FindAdaptiveHint_WhenPolicySuppressesHint_ReturnsNoHintWithSnapshot()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var context = new HintContext(2, 0);
            var result = HintFinder.FindAdaptiveHint(board, context);
            
            Assert.IsNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNull(result.Explanation);
            Assert.IsNotNull(result.BoardSnapshot);
            Assert.IsTrue(AssertBoardEqualsSnapshot(board, result.BoardSnapshot), $"{board.ToDebugString()} != {result.BoardSnapshot.ToDebugString()}");
        }

        [Test]
        public void FindBestHint_ResultSnapshotIsImmutableAfterBoardMutation()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var result = HintFinder.FindBestHint(board);

            var beforePiece1 = result.BoardSnapshot.Get(0, 1);
            var beforePiece2 = result.BoardSnapshot.Get(1, 3);
            var beforePiece3 = result.BoardSnapshot.Get(2, 2);

            var beforeBoard = board.ToDebugString();
            board.Set(0, 1, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));
            var afterBoard = board.ToDebugString();

            var afterPiece1 = result.BoardSnapshot.Get(0, 1);
            var afterPiece2 = result.BoardSnapshot.Get(1, 3);
            var afterPiece3 = result.BoardSnapshot.Get(2, 2);

            Assert.AreEqual(beforePiece1, afterPiece1);
            Assert.AreEqual(beforePiece2, afterPiece2);
            Assert.AreEqual(beforePiece3, afterPiece3);
            Assert.AreEqual(board.Get(0,1).Color, PieceColor.Red);
            Assert.AreEqual(board.Get(1,3).Color, PieceColor.Yellow);
            Assert.AreEqual(board.Get(2,2).Color, PieceColor.Blue);
            Assert.AreNotEqual(beforeBoard, afterBoard);
        }
       
        private bool AssertBoardEqualsSnapshot(BoardModel board, BoardSnapshot snapshot)
        {
            if (board.Width != snapshot.Width || board.Height != snapshot.Height)
            {
                throw new ArgumentException("Snapshot sizes do not match.");
            }

            for (int row = 0; row < board.Height; row++)
            {
                for (int col = 0; col < board.Width; col++)
                {
                    var boardPiece = board.Get(row, col);
                    var snapshotPiece = snapshot.Get(row, col);
                    if (boardPiece.Kind != snapshotPiece.Kind || boardPiece.Color != snapshotPiece.Color)
                        return false;
                }
            }
            
            return true;
        }
    }
}