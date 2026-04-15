using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public static class PieceViewMapper
    {
        public static PieceViewData Map(Piece piece)
        {
            var symbol = KindSymbol(piece) + ColorSymbol(piece);
            var color = ViewColor(piece);
            return new PieceViewData(symbol, color);
        }

        private static string KindSymbol(Piece piece)
        {
            return piece.Kind switch
            {
                PieceKind.Normal => "",
                PieceKind.RowClear => "H",
                PieceKind.ColumnClear => "V",
                PieceKind.AreaBomb => "A",
                PieceKind.ColorBomb => "CB",
                _ => ""
            };
        }

        private static string ColorSymbol(Piece piece)
        {
            if (piece.Color == null)
                return "";
            
            return piece.Color.Value switch
            {
                PieceColor.Red => "R",
                PieceColor.Green => "G",
                PieceColor.Blue => "B",
                PieceColor.Yellow => "Y",
                _ => ""
            };
        }

        private static Color ViewColor(Piece piece)
        {
            if (piece.Kind == PieceKind.Empty)
                return Color.clear;
            if(piece.Kind == PieceKind.ColorBomb)
                return Color.white;
            if(piece.Color == null)
                return Color.white;

            return piece.Color.Value switch
            {
                PieceColor.Red => Color.red,
                PieceColor.Green => Color.green,
                PieceColor.Blue => Color.cyan,
                PieceColor.Yellow => Color.yellow,
                _ => Color.white
            };
        }
    }
    
    
    
    public readonly struct PieceViewData
    {
        public string Symbol { get; }
        public Color Color { get; }

        public PieceViewData(string symbol, Color color)
        {
            Symbol = symbol;
            Color = color;
        }
    }
}