using System;
using Core.Contracts;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnPhaseTraceSnapshotGuardTest
    {
        [Test]
        public void Ctor_MutatingPhaseWithoutSnapshot_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TurnPhaseTrace(TurnPhaseType.SwapApplied));
        }

        [Test]
        public void Ctor_NonMutatingPhaseWithoutSnapshot_DoesNotThrow()
        {
            var result = new TurnPhaseTrace(TurnPhaseType.InvalidSwap);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Ctor_CascadeWaveResolvedWithSnapshotAndIndex_IsValid()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            
            var waveSnapshot = BoardSnapshotFactory.Capture(board);
            var result = new TurnPhaseTrace(TurnPhaseType.CascadeWaveResolved, cascadeIndex: 1, boardAfterPhase: waveSnapshot);
            Assert.IsNotNull(result);
        }
    }
}