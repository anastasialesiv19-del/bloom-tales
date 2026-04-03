using System;
using System.Collections.Generic;
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
            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            
            Assert.Throws<ArgumentNullException>(() =>
                new HintResult(move, HintStrength.Soft, null));
        }

        [Test]
        public void Ctor_WhenMoveIsNull_NormalizesToEmptyResult()
        {
            var reasons = new List<HintReasonType>{ HintReasonType.ActivatesSpecialDirectly };
            var explanation = new HintExplanation(reasons, 5);
            var result = new HintResult(null, HintStrength.Soft, explanation);
            
            Assert.IsNull(result.Move);
            Assert.AreEqual(HintStrength.None, result.Strength);
            Assert.IsNull(result.Explanation);
        }
    }
}