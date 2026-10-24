using Core.Matching;
using Core.Contracts;
using System;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnTraceGuardTest
    {
        [Test]
        public void Ctor_WhenPhasesIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TurnTrace(null));
        }

        [Test]
        public void TurnPhaseTrace_CascadePhaseWithoutIndex_Throws()
        {
            var snapshot = BoardSnapshotFactory.Capture(new BoardModel(1, 1));
            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.CascadeWaveResolved,
                    cascadeIndex: 0, boardAfterPhase: snapshot));
        }

        [Test]
        public void TurnPhaseTrace_NonCascadePhaseWithIndex_Throws()
        {
            var snapshot = BoardSnapshotFactory.Capture(new BoardModel(1, 1));
            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.GravityApplied,
                    cascadeIndex: 1, boardAfterPhase: snapshot));
        }

        [Test]
        public void TurnPhaseTrace_NegativeValues_Throws()
        {
            var snapshot = BoardSnapshotFactory.Capture(new BoardModel(1, 1));
            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.GravityApplied,
                    clearedCells: -1, boardAfterPhase: snapshot));

            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.PrimaryMatchResolved,
                    spawnedSpecials: -1, boardAfterPhase: snapshot));

            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.CascadeWaveResolved,
                    cascadeIndex: -1, boardAfterPhase: snapshot));
        }
    }
}
