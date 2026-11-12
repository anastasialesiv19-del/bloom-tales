using System;
using System.Collections.Generic;
using Core.Board;
using Core.Contracts;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnResultGuardTest
    {
        [Test]
        public void Ctor_WhenFailureResultHasAppliedTrue_Throws()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));

            var snapshot = BoardSnapshotFactory.Capture(board);

            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var trace = new TurnTrace(new List<TurnPhaseTrace> { new (TurnPhaseType.TurnCompleted) });
            
            Assert.Throws<ArgumentException>(() => new TurnResult(true, true, false,
                false, true, 0, 0, move, FailureReason.NoMatchCreated, trace, snapshot, snapshot));
        }

        [Test]
        public void Ctor_WhenShuffledButNotApplied_Throws()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));

            var snapshot = BoardSnapshotFactory.Capture(board);

            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var trace = new TurnTrace(new List<TurnPhaseTrace> { new (TurnPhaseType.TurnCompleted) });
            
            Assert.Throws<ArgumentException>(() => new TurnResult(true, false, true,
                false, true, 0, 0, move, FailureReason.None, trace, snapshot, snapshot));
        }

        [Test]
        public void Ctor_WhenCreatedMatchButNotApplied_Throws()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));

            var snapshot = BoardSnapshotFactory.Capture(board);

            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var trace = new TurnTrace(new List<TurnPhaseTrace> { new (TurnPhaseType.TurnCompleted) });
            
            Assert.Throws<ArgumentException>(() => new TurnResult(true, false, false,
                true, true, 0, 0, move, FailureReason.None, trace, snapshot, snapshot));
        }
    }
}