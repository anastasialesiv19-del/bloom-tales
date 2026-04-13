using System;
using Core.Progress;
using NUnit.Framework;

namespace Tests.EditMode.Progress
{
    public class PlayerProgressGuardTest
    {
        [Test]
        public void Ctor_WhenAdaptiveHintsIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PlayerProgress(null, 0));
        }

        [Test]
        public void Ctor_WhenCountersAreNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlayerProgress(new AdaptiveHintState(0, 0), -1));
        }
    }
}