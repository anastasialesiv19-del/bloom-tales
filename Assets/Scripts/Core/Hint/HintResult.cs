using Core.Board;

namespace Core.Hint
{
    public readonly struct HintResult
    {
        public Move? Move { get; }

        public HintResult(Move? move)
        {
            Move = move;
        }
    }
}