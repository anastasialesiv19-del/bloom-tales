using System;

namespace Core.Matching
{
    public readonly struct Piece
    {
        public PieceKind Kind { get; }
        public PieceColor? Color { get; }
        public bool IsEmpty => Kind == PieceKind.Empty;

        public bool IsSpecial => Kind is PieceKind.ColumnClear or PieceKind.RowClear or PieceKind.AreaBomb or PieceKind.ColorBomb;
        
        public Piece(PieceKind kind, PieceColor? color)
        {
            Kind = kind;
            if (kind == PieceKind.Empty)
            {
                if(color != null) throw new ArgumentException("This piece kind not requires a color.");
                Color = null;
            }
            else if (kind == PieceKind.ColorBomb)
            {
                if(color != null) throw new ArgumentException("This piece kind not requires a color.");
                Color = null;
            }
            else if (color != null) Color = color;
            else
                throw new ArgumentException("This piece kind requires a color.");
        }

        public static Piece Empty()
        {
            return new Piece(PieceKind.Empty, null);
        }

        public static Piece Normal(PieceColor color)
        {
            return new Piece(PieceKind.Normal, color);
        }

        public static Piece RowClear(PieceColor color)
        {
            return new Piece(PieceKind.RowClear, color);
        }

        public static Piece ColumnClear(PieceColor color)
        {
            return new Piece(PieceKind.ColumnClear, color);
        }

        public static Piece AreaBomb(PieceColor color)
        {
            return new Piece(PieceKind.AreaBomb, color);
        }

        public static Piece ColorBomb()
        {
            return new Piece(PieceKind.ColorBomb,  null);   
        }
    }
    
}