using System;

namespace Core.Matching
{
    public readonly struct Move : IEquatable<Move>
    {
        public Pos From { get; }
        public Pos To { get; }

        public Move(Pos from, Pos to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Move other)
        {
            return From.Equals(other.From) && To.Equals(other.To);
        }

        public override bool Equals(object obj)
        {
            return obj is Move other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To);
        }
    }
}