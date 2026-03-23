using System.Linq;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorTraceTest
    {
        [Test]
        public void TryProcessMove_InvalidSwap_TraceContainsInvalidSwapOnly()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));

            var move = new Move(new Pos(0, 1), new Pos(2, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            Assert.IsFalse(turnResult.WasSwapValid);
            Assert.IsFalse(turnResult.WasApplied);
            Assert.AreEqual(FailureReason.InvalidSwap, turnResult.FailureReason);
            Assert.AreEqual(1, turnResult.Trace.Phases.Count);
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.InvalidSwap));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.SwapApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.PrimaryMatchResolved));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.DirectSpecialActivation));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.GravityApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.RefillApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.CascadeWaveResolved));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.ShuffleApplied));
        }

        [Test]
        public void TryProcessMove_NoMatch_TraceContainsNoMatchCreated()
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

            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            Assert.IsFalse(turnResult.WasApplied);
            Assert.AreEqual(FailureReason.NoMatchCreated, turnResult.FailureReason);
            Assert.AreEqual(1, turnResult.Trace.Phases.Count);
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.NoMatchCreated));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.SwapApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.PrimaryMatchResolved));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.DirectSpecialActivation));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.GravityApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.RefillApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.CascadeWaveResolved));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.ShuffleApplied));
        }

        [Test]
        public void TryProcessMove_NormalMatch_TraceContainsPrimaryResolveGravityAndRefill()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.SwapApplied));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.PrimaryMatchResolved));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.GravityApplied));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.RefillApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.DirectSpecialActivation));
        }

        [Test]
        public void TryProcessMove_PrimarySpecialSpawn_TraceShowsSpawnedSpecialInPrimaryResolve()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(2, 2), new Pos(1, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            var primaryPhase = turnResult.Trace.Phases
                .Single(p => p.Type == TurnPhaseType.PrimaryMatchResolved);
            
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.PrimaryMatchResolved));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.GravityApplied));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.RefillApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.DirectSpecialActivation));
            Assert.AreEqual(1, primaryPhase.SpawnedSpecials);
        }

        [Test]
        public void TryProcessMove_DirectSpecialActivation_TraceContainsDirectActivationWithoutPrimaryMatchResolve()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.RowClear(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(2, 2), new Pos(1, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsFalse(turnResult.CreatedMatch);
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.SwapApplied));
            Assert.IsFalse(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.PrimaryMatchResolved));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.DirectSpecialActivation));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.GravityApplied));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.RefillApplied));
        }

        [Test]
        public void TryProcessMove_WithCascade_TraceContainsOnePhasePerCascadeWave()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Blue));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));

            var move = new Move(new Pos(1, 1), new Pos(2, 1));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            var cascadePhases = turnResult.Trace.Phases
                .Where(p => p.Type == TurnPhaseType.CascadeWaveResolved)
                .ToList();
            
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(turnResult.CascadeCount > 0);
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.GravityApplied));
            Assert.IsTrue(turnResult.Trace.Phases.Any(p => p.Type == TurnPhaseType.CascadeWaveResolved));
            Assert.AreEqual(turnResult.CascadeCount, cascadePhases.Count);
            
            for (int i = 0; i < cascadePhases.Count; i++)
            {
                Assert.AreEqual(i + 1, cascadePhases[i].CascadeIndex);
                Assert.Greater(cascadePhases[i].ClearedCells, 0);
            }
        }

        [Test]
        [Ignore("сценарій WasShuffled == true потребує контрольованого refill з поточним random цей тест крихкий.")]
        public void TryProcessMove_ShuffleAfterResolution_TraceContainsShuffleApplied()
        {
            
        }
    }
}