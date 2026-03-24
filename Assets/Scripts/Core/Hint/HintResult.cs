using Core.Matching;
using JetBrains.Annotations;

namespace Core.Hint
{
    public readonly struct HintResult
    {
        public Move? Move { get; }
        public  HintStrength Strength { get; }
        [CanBeNull] public HintExplanation Explanation { get; }

        public HintResult(Move? move)
        {
            Move = move;
            Strength = HintStrength.None;
            Explanation = null;
        }
        
        public HintResult(Move? move,  HintStrength strength, HintExplanation explanation)
        {
            Move = move;
            Strength = move == null ? HintStrength.None : strength;
            Explanation = move == null ? null : explanation;
        }
    }
}