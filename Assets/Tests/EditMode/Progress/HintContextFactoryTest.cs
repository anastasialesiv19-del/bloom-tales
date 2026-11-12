using System;
using Core.Hint;
using Core.Progress;
using NUnit.Framework;

namespace Tests.EditMode.Progress
{
    public class HintContextFactoryTest
    {
        [Test]
        public void Create_FromIdleSecondsAndAdaptiveState_MapsExpectedValues()
        {
            var context = HintContextFactory.Create(8f, new AdaptiveHintState(2, 4));
            
            Assert.AreEqual(8f, context.IdleSeconds);
            Assert.AreEqual(2, context.ConsecutiveHintsShown);
        }

        [Test]
        public void Create_WhenStateIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => HintContextFactory.Create(5f, null));
        }
    }
}