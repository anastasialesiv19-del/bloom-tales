using System.Collections.Generic;
using Core.Board;

namespace Core.Match
{
    public class Match
    {
        public IReadOnlyList<Pos> Cells { get; }

        public Match(List<Pos> cells)
        {
            Cells = cells;
        }
    }
}