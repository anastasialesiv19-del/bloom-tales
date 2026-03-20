using System.Collections.Generic;
using System.Linq;

namespace Core.Matching
{
    public static class SpecialEffectResolver
    {
        public static IReadOnlyCollection<Pos> CollectTriggeredCells(BoardModel board,
            IReadOnlyCollection<Pos> initiallyClearedCells)
        {
            var allCleared =  new HashSet<Pos>(initiallyClearedCells);
            var processedSpecials =  new HashSet<Pos>();
            bool changed =  true;
            while (changed)
            {
                changed = false;
                foreach (var cell in allCleared.ToList())
                {
                    var piece = board.Get(cell.Row, cell.Col);
                    
                    if (!IsTriggerableSpecial(piece.Kind)) continue;
                    if (processedSpecials.Contains(cell)) continue;
                    
                    var effectCells = GetEffectCells(board, cell, piece);
                    processedSpecials.Add(cell);
                    foreach (var effectCell in effectCells)
                    {
                        if (allCleared.Add(effectCell))
                        {
                            changed = true;
                        }
                    }
                }
            }
            return allCleared;
        }

        private static bool IsTriggerableSpecial(PieceKind kind)
        {
            if(kind == PieceKind.RowClear || 
               kind == PieceKind.ColumnClear || 
               kind == PieceKind.AreaBomb) return true;

            return false;
        }
        
        private static IEnumerable<Pos> GetEffectCells(BoardModel board, Pos cell, Piece piece)
        {
            switch (piece.Kind)
            {
                case PieceKind.RowClear:
                    return GetRowCells(board, cell.Row);
                case PieceKind.ColumnClear:
                    return GetColumnCells(board, cell.Col);
                case PieceKind.AreaBomb:
                    return GetAresCells(board, cell);
                default:
                    return Enumerable.Empty<Pos>();
            }
        }

        private static IEnumerable<Pos> GetAresCells(BoardModel board, Pos cell)
        {
            for (int row = cell.Row - 1; row <= cell.Row + 1; row++)
            {
                for (int col = cell.Col - 1; col <= cell.Col + 1; col++)
                {
                    if (board.InBounds(row, col))
                    {
                        yield return new Pos(row, col);
                    }
                }
            }
        }

        private static IEnumerable<Pos> GetColumnCells(BoardModel board, int cellCol)
        {
            for(int row = 0;  row < board.Height; row++)
                yield return new Pos(row, cellCol);
        }

        private static IEnumerable<Pos> GetRowCells(BoardModel board, int cellRow)
        {
            for(int col = 0;  col < board.Width; col++)
                yield return new Pos(cellRow, col);
        }
    }
}