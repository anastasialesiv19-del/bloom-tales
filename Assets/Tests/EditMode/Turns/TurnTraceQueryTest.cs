using System;
using System.Collections.Generic;
using Core.Board;
using Core.Contracts;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnTraceQueryTest
    {
        [Test]
        public void HasPhase_WhenPhaseExists_ReturnsTrue()
        {
            var board1 = new BoardModel(2, 2);
            
            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            
            var snapshot1 = BoardSnapshotFactory.Capture(board1);
            
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.SwapApplied, boardAfterPhase: snapshot1),
                new(TurnPhaseType.GravityApplied, boardAfterPhase: snapshot1),
                new(TurnPhaseType.TurnCompleted)
            };

            var trace = new TurnTrace(phases);
            
            Assert.IsTrue(TurnTraceQueries.HasPhase(trace, TurnPhaseType.GravityApplied));
        }

        [Test]
        public void HasPhase_WhenPhaseMissing_ReturnsFalse()
        {
            var board1 = new BoardModel(2, 2);
            
            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            
            var snapshot1 = BoardSnapshotFactory.Capture(board1);
            
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.SwapApplied, boardAfterPhase: snapshot1),
                new(TurnPhaseType.GravityApplied, boardAfterPhase: snapshot1),
                new(TurnPhaseType.TurnCompleted)
            };

            var trace = new TurnTrace(phases);
            
            Assert.IsFalse(TurnTraceQueries.HasPhase(trace, TurnPhaseType.ShuffleApplied));
        }

        [Test]
        public void TryGetPhase_WhenPhaseExists_ReturnsFirstMatchingPhase()
        {
            var board1 = new BoardModel(2, 2);
            
            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            
            var snapshot1 = BoardSnapshotFactory.Capture(board1);
            
            var board2 = new BoardModel(2, 2);
            
            board2.Set(0, 0, Piece.Normal(PieceColor.Green));
            board2.Set(0, 1, Piece.Normal(PieceColor.Red));
            board2.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(1, 1, Piece.Normal(PieceColor.Red));
            
            var snapshot2 = BoardSnapshotFactory.Capture(board2);
            
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.SwapApplied, boardAfterPhase: snapshot1),
                new(TurnPhaseType.CascadeWaveResolved, cascadeIndex: 1, boardAfterPhase: snapshot1),
                new(TurnPhaseType.CascadeWaveResolved, cascadeIndex: 2, boardAfterPhase: snapshot2),
                new(TurnPhaseType.TurnCompleted)
            };
            
            var trace = new TurnTrace(phases);
            var result = TurnTraceQueries.TryGetPhase(trace, TurnPhaseType.CascadeWaveResolved, out var phase);
            
            Assert.IsTrue(result);
            Assert.AreEqual(TurnPhaseType.CascadeWaveResolved, phase.Type);
            Assert.AreEqual(1, phase.CascadeIndex);
            Assert.IsNotNull(phase.BoardAfterPhase);
        }

        [Test]
        public void TryGetPhase_WhenPhaseMissing_ReturnsFalse()
        {
            var board1 = new BoardModel(2, 2);
            
            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            
            var snapshot1 = BoardSnapshotFactory.Capture(board1);
            
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.SwapApplied,  boardAfterPhase: snapshot1),
                new(TurnPhaseType.PrimaryMatchResolved,  boardAfterPhase: snapshot1),
                new(TurnPhaseType.TurnCompleted)
            };

            var trace = new TurnTrace(phases);
            
            Assert.IsFalse(TurnTraceQueries.TryGetPhase(trace, TurnPhaseType.DirectSpecialActivation, out var phase));
        }
        
        [Test]
        public void GetCascadePhases_ReturnsOnlyCascadePhasesInOrder()
        {
            var board1 = new BoardModel(2, 2);
            
            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            
            var snapshot1 = BoardSnapshotFactory.Capture(board1);
            
            var board2 = new BoardModel(2, 2);
            
            board2.Set(0, 0, Piece.Normal(PieceColor.Green));
            board2.Set(0, 1, Piece.Normal(PieceColor.Red));
            board2.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(1, 1, Piece.Normal(PieceColor.Red));
            
            var snapshot2 = BoardSnapshotFactory.Capture(board2);
            
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.SwapApplied, boardAfterPhase: snapshot1),
                new(TurnPhaseType.PrimaryMatchResolved, boardAfterPhase: snapshot1),
                new(TurnPhaseType.CascadeWaveResolved, cascadeIndex: 1, boardAfterPhase: snapshot1),
                new(TurnPhaseType.GravityApplied,  boardAfterPhase: snapshot1),
                new(TurnPhaseType.CascadeWaveResolved, cascadeIndex: 2, boardAfterPhase: snapshot2),
                new(TurnPhaseType.TurnCompleted)
            };
            
            var trace = new TurnTrace(phases);
            var cascades = TurnTraceQueries.GetCascadePhases(trace);
            
            Assert.AreEqual(2, cascades.Count);
            Assert.AreEqual(1,cascades[0].CascadeIndex);
            Assert.AreEqual(2,cascades[1].CascadeIndex);
            Assert.AreEqual(TurnPhaseType.CascadeWaveResolved,cascades[0].Type);
            Assert.AreEqual(TurnPhaseType.CascadeWaveResolved,cascades[1].Type);
        }

        [Test]
        public void TryGetLatestBoardSnapshot_WhenMutatingPhaseExists_ReturnsLastSnapshot()
        {
            var board1 = new BoardModel(2, 2);
            
            board1.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board1.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board1.Set(1, 0, Piece.Normal(PieceColor.Green));
            board1.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            
            var snapshot1 = BoardSnapshotFactory.Capture(board1);
            
            var board2 = new BoardModel(2, 2);
            
            board2.Set(0, 0, Piece.Normal(PieceColor.Green));
            board2.Set(0, 1, Piece.Normal(PieceColor.Red));
            board2.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board2.Set(1, 1, Piece.Normal(PieceColor.Red));
            
            var snapshot2 = BoardSnapshotFactory.Capture(board2);
            
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.SwapApplied, boardAfterPhase: snapshot1),
                new(TurnPhaseType.GravityApplied,  boardAfterPhase: snapshot2),
                new(TurnPhaseType.TurnCompleted)
            };
            
            var trace = new TurnTrace(phases);
            var result = TurnTraceQueries.TryGetLatestBoardSnapshot(trace, out var snapshot);
            
            Assert.IsTrue(result);
            Assert.IsNotNull(snapshot);
            Assert.IsTrue(AssertSnapshotsEqual(snapshot2, snapshot));
        }

        [Test]
        public void TryGetLatestBoardSnapshot_WhenNoSnapshotPhaseExists_ReturnsFalse()
        {
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.InvalidSwap),
                new(TurnPhaseType.NoMatchCreated),
                new(TurnPhaseType.TurnCompleted)
            };
            
            var trace = new TurnTrace(phases);
            var result = TurnTraceQueries.TryGetLatestBoardSnapshot(trace, out var snapshot);
            
            Assert.IsFalse(result);
            Assert.IsNull(snapshot);
        }

        [Test]
        public void GetLatestBoardSnapshot_WhenNoSnapshotExists_Throws()
        {
            var phases = new List<TurnPhaseTrace>
            {
                new(TurnPhaseType.InvalidSwap),
                new(TurnPhaseType.NoMatchCreated),
                new(TurnPhaseType.TurnCompleted)
            };
            
            var trace = new TurnTrace(phases);
            Assert.Throws<InvalidOperationException>(() => TurnTraceQueries.GetLatestBoardSnapshot(trace));
        }
        
        private bool AssertSnapshotsEqual(BoardSnapshot expectedSnapshot, BoardSnapshot actualSnapshot)
        {
            if (expectedSnapshot.Width != actualSnapshot.Width || expectedSnapshot.Height != actualSnapshot.Height)
            {
                throw new ArgumentException("Snapshot sizes do not match.");
            }

            for (var row = 0; row < expectedSnapshot.Height; row++)
            {
                for (var col = 0; col < expectedSnapshot.Width; col++)
                {
                    var expectedPiece = expectedSnapshot.Get(row, col);
                    var actualPiece = actualSnapshot.Get(row, col);
                    if (expectedPiece.Kind != actualPiece.Kind || expectedPiece.Color != actualPiece.Color)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}