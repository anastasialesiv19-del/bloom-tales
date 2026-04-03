using System;
using System.Collections.Generic;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnResultGuardTest
    {
        [Test]
        public void Ctor_WhenFailureResultHasAppliedTrue_Throws()
        {
            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var trace = new TurnTrace(new List<TurnPhaseTrace> { new TurnPhaseTrace(TurnPhaseType.TurnCompleted) });
            Assert.Throws<ArgumentException>(() => new TurnResult(true, true, false, 
                false, true, 0, 0, move, FailureReason.NoMatchCreated, trace) );
        }

        [Test]
        public void Ctor_WhenShuffledButNotApplied_Throws()
        {
            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var trace = new TurnTrace(new List<TurnPhaseTrace> { new TurnPhaseTrace(TurnPhaseType.TurnCompleted) });
            Assert.Throws<ArgumentException>(() => new TurnResult(true, false, true, 
                false, true, 0, 0, move, FailureReason.None, trace) );
        }

        [Test]
        public void Ctor_WhenCreatedMatchButNotApplied_Throws()
        {
            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var trace = new TurnTrace(new List<TurnPhaseTrace> { new TurnPhaseTrace(TurnPhaseType.TurnCompleted) });
            Assert.Throws<ArgumentException>(() => new TurnResult(true, false, false, 
                true, true, 0, 0, move, FailureReason.None, trace) );
        }
    }
}