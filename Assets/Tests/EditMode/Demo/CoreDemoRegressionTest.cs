using System.Linq;
using Core.Board;
using Core.Contracts;
using Core.Generation;
using Core.Hint;
using Core.Turns;
using NUnit.Framework;
using Tests.EditMode.TestDoubles;

namespace Tests.EditMode.Demo
{
    public class CoreDemoRegressionTest
    {
        [Test]
        public void Demo_NormalMatchTurn_RemainsStable()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasSwapValid);
            Assert.IsTrue(result.WasApplied);
            Assert.IsTrue(result.CreatedMatch);
            Assert.Greater(result.TotalClearedCells, 0);
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.SwapApplied));
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.PrimaryMatchResolved));
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.GravityApplied));
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.RefillApplied));
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.TurnCompleted));
            Assert.IsNotNull(result.FinalBoardSnapshot);
            Assert.AreEqual(board.ToDebugString(), result.FinalBoardSnapshot.ToDebugString());
        }

        [Test]
        public void Demo_PrimarySpecialSpawn_RemainsStable()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(0, 2), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
            var hasPrimaryMatchPhase = TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.PrimaryMatchResolved, out var phase);

            Assert.IsTrue(result.WasApplied);
            Assert.IsTrue(result.CreatedMatch);
            Assert.Greater(result.TotalClearedCells, 0);
            Assert.IsTrue(hasPrimaryMatchPhase);
            Assert.IsTrue(phase.SpawnedSpecials > 0);
            Assert.IsNotNull(phase.BoardAfterPhase);

            var hasAnySpecial = Enumerable.Range(0, phase.BoardAfterPhase.Height).SelectMany(row => Enumerable.Range(0, phase.BoardAfterPhase.Width)
                    .Select(col => phase.BoardAfterPhase.Get(row, col)))
                    .Any(piece => piece.Kind is PieceKind.RowClear or PieceKind.ColumnClear or PieceKind.AreaBomb or PieceKind.ColorBomb);

            Assert.IsTrue(hasAnySpecial);
        }

        [Test]
        public void Demo_DirectSpecialActivation_RemainsStable()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.RowClear(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(0, 2), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
            var hasDirectActivationPhase = TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.DirectSpecialActivation, out var phase);

            Assert.IsTrue(result.WasApplied);
            Assert.IsFalse(result.CreatedMatch);
            Assert.Greater(result.TotalClearedCells, 0);
            Assert.IsTrue(hasDirectActivationPhase);
            Assert.IsTrue(phase.ClearedCells > 0);
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.GravityApplied));
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.RefillApplied));
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.TurnCompleted));
            Assert.IsNotNull(result.FinalBoardSnapshot);
        }

        [Test]
        public void Demo_CascadeTurn_RemainsStable()
        {
            var result = CascadeTestScenario.ResolveTwoWaves();
            
            Assert.IsTrue(result.WasApplied);
            Assert.IsTrue(result.CreatedMatch);
            Assert.AreEqual(2, result.CascadeCount);
            
            var phases = TurnTraceQueries.GetCascadePhases(result.Trace);
            
            Assert.AreEqual(2, phases.Count);
            
            for (var i = 0; i < 2; i++)
            {
                Assert.AreEqual(i + 1, phases[i].CascadeIndex);
                Assert.AreEqual(3, phases[i].ClearedCells);
                Assert.IsNotNull(phases[i].BoardAfterPhase);
            }
        }

        [Test]
        public void Demo_ShuffleAfterResolution_RemainsStable()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.ColorBomb());
            board.Set(0, 1, Piece.ColorBomb());
            board.Set(0, 2, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));

            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var provider = new SequenceRandomProvider(new[] { 0, 0, 1, 0, 0, 1, 1, 3, 3, 0, 0, 0, 0, 0, 0, 0, 1 });
            var result = TurnProcessor.TryProcessMove(board, move, provider);
            var hasShuffleResolverPhase = TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.ShuffleApplied, out var phase);

            Assert.IsTrue(result.WasApplied);
            Assert.IsTrue(result.WasShuffled);
            Assert.IsTrue(hasShuffleResolverPhase);
            Assert.IsNotNull(phase.BoardAfterPhase);
            Assert.IsNotNull(result.FinalBoardSnapshot);
            Assert.AreEqual(board.ToDebugString(), result.FinalBoardSnapshot.ToDebugString());
            Assert.IsTrue(BoardShuffler.IsPlayableBoard(board));
        }

        [Test]
        public void Demo_HintContract_RemainsStable()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var context = new HintContext(5, 0);
            var best = HintFinder.FindBestHint(board);
            var adaptive = HintFinder.FindAdaptiveHint(board, context);

            Assert.IsNotNull(best.Move);
            Assert.IsNotNull(best.BoardSnapshot);
            Assert.AreEqual(HintStrength.None, best.Strength);
            Assert.IsNotNull(adaptive.Move);
            Assert.AreEqual(adaptive.Move, best.Move);
            Assert.AreEqual(adaptive.BoardSnapshot.ToDebugString(), best.BoardSnapshot.ToDebugString());
            Assert.AreEqual(adaptive.Explanation.Score, best.Explanation.Score);

        }

        [Test]
        public void Demo_FailedTurn_DoesNotMutateBoard()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Red));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var before = BoardSnapshotFactory.Capture(board);
            var move = new Move(new Pos(1, 2), new Pos(0, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(result.WasSwapValid);
            Assert.IsFalse(result.WasApplied);
            Assert.AreEqual(FailureReason.NoMatchCreated, result.FailureReason);
            Assert.IsTrue(TurnTraceQueries.HasPhase(result.Trace, TurnPhaseType.NoMatchCreated));
            Assert.AreEqual(before.ToDebugString(), BoardSnapshotFactory.Capture(board).ToDebugString());
            Assert.IsNotNull(result.FinalBoardSnapshot);
        }
    }
}
