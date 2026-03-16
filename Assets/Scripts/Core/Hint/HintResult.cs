using Core.Board;

namespace Core.Hint
{
    public readonly struct HintResult
    {
        public Move? Move { get; }
        public  HintStrength Strength { get; }

        public HintResult(Move? move)
        {
            Move = move;
            Strength = HintStrength.None;
        }
        
        public HintResult(Move? move,  HintStrength strength)
        {
            Move = move;
            Strength = move == null ? HintStrength.None : strength;
        }
    }
}