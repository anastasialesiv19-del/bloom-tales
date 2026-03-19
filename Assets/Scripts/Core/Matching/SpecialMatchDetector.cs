using System.Collections.Generic;
using Core.Matching;

namespace Core.Matching
{
    public static class SpecialMatchDetector
    {
        public static IReadOnlyList<SpecialMatchResult> Detect(BoardModel board)
        {
            var matches = MatchFinder.FindMatches(board);
            var result = new List<SpecialMatchResult>();
            foreach (var match in matches)
            {
                var type = ClassifyMatch(match);
                if(type != SpecialMatchType.None)
                    result.Add(new SpecialMatchResult(type, match.Cells));
            }
            return result;
        }

        public static SpecialMatchType ClassifyMatch(Match match)
        {
            int cells = match.Cells.Count;
            if (cells <= 3) return SpecialMatchType.None;
            
            switch (cells)
            {
                case 4:
                    return IsHorizontalLine(match.Cells) || IsVerticalLine(match.Cells) ? SpecialMatchType.Line4 : SpecialMatchType.None;
                case 5:
                    if(IsHorizontalLine(match.Cells) || IsVerticalLine(match.Cells)) return SpecialMatchType.Line5;
                    if(IsTShape(match.Cells)) return SpecialMatchType.TShape;
                    return SpecialMatchType.LShape;
                default:
                    return SpecialMatchType.None;
            }
        }

        public static bool IsHorizontalLine(IReadOnlyList<Pos> cells)
        {
            var firstRow = cells[0].Row;
            for (int index = 1; index < cells.Count; index++)
                if (cells[index].Row != firstRow) return false;
            return true;
        }
        private static bool IsVerticalLine(IReadOnlyList<Pos> cells)
        {
            var firstCol = cells[0].Col;
            for (int index = 1; index < cells.Count; index++)
                if (cells[index].Col != firstCol) return false;
            return true;
        }
        private static bool IsTShape(IReadOnlyList<Pos> cells)
        {
            var otherCells = new HashSet<Pos>(cells);
            foreach (var cell in cells)
            {
                int neighborCount = 0;
                foreach (var otherCell in otherCells)
                {
                    if (otherCell.Row == cell.Row && otherCell.Col == cell.Col) continue;
                    if(MoveValidator.AreAdjacent(cell, otherCell))
                        neighborCount++;
                }
                if (neighborCount == 3) return true;
            }
            return false;
        }
    }
}