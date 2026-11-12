using System;
using System.Collections.Generic;
using Core.Board;
using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintGoalContextTest
    {
        [Test]
        public void Constructor_WhenRemainingScoreProvided_StoresValue()
        {
            var context = new HintGoalContext(new Dictionary<PieceColor, int>(), 0, 12);
            
            Assert.AreEqual(12, context.RemainingScore);
        }
        
        [Test]
        public void Constructor_WhenRemainingScoreOmitted_DefaultsToZero()
        {
            var context = new HintGoalContext(new Dictionary<PieceColor, int>(), 0);
            
            Assert.AreEqual(0, context.RemainingScore);
        }

        [Test]
        public void Empty_HasZeroRemainingScore()
        {
            Assert.AreEqual(0, HintGoalContext.Empty.RemainingScore);
        }

        [Test]
        public void Constructor_WhenRemainingScoreNegative_Throws()
        {
                Assert.Throws<ArgumentOutOfRangeException>(() => new HintGoalContext(new Dictionary<PieceColor, int>(), 0, -1));
        }
    }
}