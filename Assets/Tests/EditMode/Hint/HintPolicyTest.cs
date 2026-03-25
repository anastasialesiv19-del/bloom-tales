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
            var strength = HintPolicy.GetStrength(context, 0);
            Assert.AreEqual(HintStrength.None, strength);
        }

        [Test]
        public void GetStrength_ReturnsSoft_ForMediumIdle()
        {
            var context = new HintContext(5, 0);
            var strength = HintPolicy.GetStrength(context, 0);
            Assert.AreEqual(HintStrength.Soft, strength);
        }

        [Test]
        public void GetStrength_ReturnsStrong_ForLongIdle()
        {
            var context = new HintContext(9, 0);
            var strength = HintPolicy.GetStrength(context, 0);
            Assert.AreEqual(HintStrength.Strong, strength);
        }
        
        [Test]
        public void GetStrength_SoftBaseAndHighScore_ReturnsStrong()
        {
            var context = new HintContext(5, 0);
            var strength = HintPolicy.GetStrength(context, 5);
            Assert.AreEqual(HintStrength.Strong, strength);
        }
        
        [Test]
        public void GetStrength_SoftBaseAndLowScore_ReturnsSoft()
        {
            var context = new HintContext(5, 0);
            var strength = HintPolicy.GetStrength(context, 3);
            Assert.AreEqual(HintStrength.Soft, strength);
        }
        
        [Test]
        public void GetStrength_NoneBase_EvenWithHighScore_ReturnsNone()
        {
            var context = new HintContext(2, 0);
            var strength = HintPolicy.GetStrength(context, 999);
            Assert.AreEqual(HintStrength.None, strength);
        }
    }
}