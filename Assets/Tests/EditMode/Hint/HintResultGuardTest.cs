using System;
using System.Collections.Generic;
using Core.Contracts;
using Core.Hint;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintResultGuardTest
    {
        [Test]
        public void Ctor_WhenMoveExistsAndExplanationIsNull_Throws()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));

            var snapshot = BoardSnapshotFactory.Capture(board);
            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            
            Assert.Throws<ArgumentNullException>(() =>
                new HintResult(move, HintStrength.Soft, null, snapshot));
        }

        [Test]
        public void Ctor_WhenMoveIsNull_NormalizesToEmptyResultButKeepsSnapshot()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));

            var snapshot = BoardSnapshotFactory.Capture(board);
            var reasons = new List<HintReasonType>{ HintReasonType.ActivatesSpecialDirectly };
            var explanation = new HintExplanation(reasons, 5);
            var result = new HintResult(null, HintStrength.Soft, explanation, snapshot);
            
            Assert.IsNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNull(result.Explanation);
            Assert.IsNotNull(result.BoardSnapshot);
            Assert.IsTrue(AssertSnapshotsEqual(snapshot, result.BoardSnapshot));
        }

        [Test]
        public void Ctor_WhenSnapshotIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new HintResult(null, HintStrength.None, null, null));
            
            Assert.Throws<ArgumentNullException>(() => 
                new HintResult(null, null));
            
            var reasons = new List<HintReasonType>{ HintReasonType.ActivatesSpecialDirectly };
            var explanation = new HintExplanation(reasons, 5);
            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            Assert.Throws<ArgumentNullException>(() =>
                new HintResult(move, HintStrength.Soft, explanation, null));
            
        }
        
         
        private bool AssertSnapshotsEqual(BoardSnapshot expectedSnapshot, BoardSnapshot actualSnapshot)
        {
            if (expectedSnapshot.Width != actualSnapshot.Width || expectedSnapshot.Height != actualSnapshot.Height)
                throw new ArgumentException("Snapshot sizes do not match.");
            for (int row = 0; row < expectedSnapshot.Height; row++)
            for (int col = 0; col < expectedSnapshot.Width; col++)
            {
                var expectedPiece = expectedSnapshot.Get(row, col);
                var actualPiece = actualSnapshot.Get(row, col);
                if (expectedPiece.Kind != actualPiece.Kind || expectedPiece.Color != actualPiece.Color)
                    return false;
            }

            return true;
        }

    }
}