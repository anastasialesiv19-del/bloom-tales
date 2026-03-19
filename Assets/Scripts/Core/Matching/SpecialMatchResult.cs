using System.Collections.Generic;
using Core.Matching;

namespace Core.Matching
{
    public readonly struct SpecialMatchResult
    {
        public SpecialMatchType Type { get; }
        public IReadOnlyList<Pos> Cells { get; }
        public int Size { get; }

        public SpecialMatchResult(SpecialMatchType type, IReadOnlyList<Pos> cells)
        {
            Type = type;
            Cells = cells;
            Size = cells.Count;
        }
    }
}