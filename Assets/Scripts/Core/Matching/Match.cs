using System.Collections.Generic;
using Core.Board;

namespace Core.Matching
{
    public class Match
    {
        private readonly List<Pos> _cells;
        public IReadOnlyList<Pos> Cells { get; }

        public Match(List<Pos> cells)
        {
            _cells = cells;
            Cells = _cells;
        }
    }
}