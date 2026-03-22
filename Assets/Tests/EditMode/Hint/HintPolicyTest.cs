using Core.Hint;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintPolicyTest
    {
        [Test]
        public void GetStrength_ReturnsNone_ForShortIdle()
        {
            var context = new HintContext(2, 0);
            var strength = HintPolicy.GetStrength(context);
            Assert.AreEqual(HintStrength.None, strength);
        }

        [Test]
        public void GetStrength_ReturnsSoft_ForMediumIdle()
        {
            var context = new HintContext(5, 0);
            var strength = HintPolicy.GetStrength(context);
            Assert.AreEqual(HintStrength.Soft, strength);
        }

        [Test]
        public void GetStrength_ReturnsStrong_ForLongIdle()
        {
            var context = new HintContext(9, 0);
            var strength = HintPolicy.GetStrength(context);
            Assert.AreEqual(HintStrength.Strong, strength);
        }
    }
}