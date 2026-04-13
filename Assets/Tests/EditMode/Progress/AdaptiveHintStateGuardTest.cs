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
            Assert.Throws<InvalidOperationException>(() => new AdaptiveHintState(0, -1));
        }
    }
}