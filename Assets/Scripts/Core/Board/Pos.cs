using System;
using JetBrains.Annotations;

namespace Core.Board
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
    }
}