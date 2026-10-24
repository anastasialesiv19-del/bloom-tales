using System;
using Core.Board;
using Core.Generation;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Generation
{
    public class BoardShufflerInvariantTest
    {
        [Test]
        public void Shuffle_WhenBoardHasEmptyCells_ThrowsInvalidOperationException()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Empty());
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

            Assert.Throws<InvalidOperationException>(() => BoardShuffler.Shuffle(board));
        }

        [Test]
        public void IsPlayableBoard_WhenBoardContainsMatches_ReturnsFalse()
        {
            var board = new BoardModel(3, 3);
            var colors = new[,]
            {
                { PieceColor.Red, PieceColor.Red, PieceColor.Red },
                { PieceColor.Blue, PieceColor.Green, PieceColor.Blue },
                { PieceColor.Green, PieceColor.Blue, PieceColor.Yellow }
            };
            foreach (var pos in board.AllPositions())
                board.Set(pos.Row, pos.Col, Piece.Normal(colors[pos.Row, pos.Col]));
            Assert.IsFalse(BoardResolver.HasEmptyCells(board));
            Assert.Greater(MatchFinder.FindMatches(board).Count, 0);
            Assert.IsTrue(MoveFinder.HasAnyPossibleMove(board));
            Assert.IsFalse(BoardShuffler.IsPlayableBoard(board));
        }

        [Test]
        public void IsPlayableBoard_WhenBoardHasNoPossibleMoves_ReturnsFalse()
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

            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            bool deadlock = MoveFinder.IsDeadlocked(board);

            Assert.IsFalse(isPlayable);
            Assert.IsTrue(deadlock);
        }

        [Test]
        public void Shuffle_OnFullBoard_ProducesPlayableBoardWithoutEmptyCells()
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

            BoardShuffler.Shuffle(board);

            Assert.IsFalse(BoardResolver.HasEmptyCells(board));
            Assert.IsTrue(BoardShuffler.IsPlayableBoard(board));
        }
    }
}
