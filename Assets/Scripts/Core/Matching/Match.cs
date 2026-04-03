using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Matching
{
    public class Match
    {
        private readonly List<Pos> _cells;
        public IReadOnlyList<Pos> Cells { get; }

        public Match(List<Pos> cells)
        {
            if(cells == null)
                throw new ArgumentNullException(nameof(cells));
            if(cells.Count == 0)
                throw new ArgumentException("Cells must have at least one cell.", nameof(cells));
            _cells = cells;
            Cells = _cells;
        }
    }
}