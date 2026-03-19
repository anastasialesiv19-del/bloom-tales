using System;
using JetBrains.Annotations;

namespace Core.Matching
{
    public readonly struct Pos
    {
        public int Row { get; }
        public int Col { get; }
        public Pos(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool Equals(Pos other)
        {
            return Row == other.Row && Col == other.Col;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if(obj is Pos)
                return Equals((Pos) obj);
            return false;
            
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }
    }
}