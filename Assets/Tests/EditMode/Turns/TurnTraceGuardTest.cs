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
            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.CascadeWaveResolved, cascadeIndex: 0));
        }

        [Test]
        public void TurnPhaseTrace_NonCascadePhaseWithIndex_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.GravityApplied, cascadeIndex: 1)); 
        }

        [Test]
        public void TurnPhaseTrace_NegativeValues_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.GravityApplied, clearedCells: -1));

            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.PrimaryMatchResolved, spawnedSpecials: -1));

            Assert.Throws<ArgumentException>(() =>
                new TurnPhaseTrace(TurnPhaseType.CascadeWaveResolved, cascadeIndex: -1));
        }
    }
}