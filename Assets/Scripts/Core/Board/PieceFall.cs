using Core.Matching;

namespace Core.Board
{
    public readonly struct PieceFall
    {
        public Pos From { get; }
        public Pos To { get; }

        public PieceFall(Pos from, Pos to)
        {
            From = from;
            To = to;
        }
    }
}