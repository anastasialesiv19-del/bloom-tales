namespace Core.Board
{
    public readonly struct Move
    {
        public Pos From { get; }
        public Pos To { get; }

        public Move(Pos from, Pos to)
        {
            From = from;
            To = to;
        }
    }
}