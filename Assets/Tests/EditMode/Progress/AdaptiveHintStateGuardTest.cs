using System;
using Core.Progress;
using NUnit.Framework;

namespace Tests.EditMode.Progress
{
    public class AdaptiveHintStateGuardTest
    {
        [Test]
        public void Ctor_WhenValuesAreNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AdaptiveHintState(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AdaptiveHintState(-1, 0));
        }
    }
}