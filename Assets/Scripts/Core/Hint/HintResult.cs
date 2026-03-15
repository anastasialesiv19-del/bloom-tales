using Core.Board;

namespace Core.Hint
{
    public readonly struct HintResult
    {
        public bool HasHint { get; }
        public Move? Move { get; }

        public HintResult(bool hasHint, Move? move)
        {
            HasHint = hasHint;
            Move = move;
        }
    }
}