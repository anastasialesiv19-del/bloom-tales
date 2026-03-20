using System;
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
                    
                    var effectCells = GetEffectCells(board, cell, piece.Kind, initiallyClearedCells.ToList());
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
            if(kind is PieceKind.RowClear or 
               PieceKind.ColumnClear or 
               PieceKind.AreaBomb or 
               PieceKind.ColorBomb) return true;
            return false;
        }
        
        private static IEnumerable<Pos> GetEffectCells(BoardModel board, Pos cell, PieceKind kind, List<Pos> initiallyClearedCells)
        {
            switch (kind)
            {
                case PieceKind.RowClear:
                    return GetRowCells(board, cell.Row);
                case PieceKind.ColumnClear:
                    return GetColumnCells(board, cell.Col);
                case PieceKind.AreaBomb:
                    return GetAreaCells(board, cell);
                case PieceKind.ColorBomb:
                    var color = ResolveColorBombTarget(board, cell, initiallyClearedCells);
                    return GetAllCellsOfColor(board, color);
                default:
                    return Enumerable.Empty<Pos>();
            }
        }

        private static IEnumerable<Pos> GetRowCells(BoardModel board, int cellRow)
        {
            for(int col = 0;  col < board.Width; col++)
                yield return new Pos(cellRow, col);
        }
        
        private static IEnumerable<Pos> GetColumnCells(BoardModel board, int cellCol)
        {
            for(int row = 0;  row < board.Height; row++)
                yield return new Pos(row, cellCol);
        }
        
        private static IEnumerable<Pos> GetAreaCells(BoardModel board, Pos cell)
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

        private static PieceColor ResolveColorBombTarget(BoardModel board, Pos cell, List<Pos> initiallyClearedCells)
        {
            foreach (var pos in initiallyClearedCells)
            {
                if (MoveValidator.AreAdjacent(cell, pos))
                {
                    var pieceColor = board.Get(pos.Row, pos.Col).Color;
                    if (pieceColor != null)
                        return (PieceColor)pieceColor;
                }
            }
            throw new InvalidOperationException("At least one neighbor must have a color.");
        } 
        private static IEnumerable<Pos> GetAllCellsOfColor(BoardModel board, PieceColor color)
        {
            foreach (var pos in board.AllPositions())
            {
                if(board.Get(pos.Row, pos.Col).Color == color)
                    yield return new Pos(pos.Row, pos.Col);
            }
        }
    }
}