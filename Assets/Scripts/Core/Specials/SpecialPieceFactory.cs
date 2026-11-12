using System;
using Core.Board;
using Core.Matching;

namespace Core.Specials
{
    public static class SpecialPieceFactory
    {
        public static Piece CreateFromMatchType(SpecialMatchType matchType, PieceColor color, bool preferHorizontal)
        {
            switch (matchType)
            {
                case SpecialMatchType.Line4:
                    return preferHorizontal ? Piece.RowClear(color) : Piece.ColumnClear(color);
                case SpecialMatchType.Line5:
                    return Piece.ColorBomb();
                case SpecialMatchType.TShape:
                case SpecialMatchType.LShape:
                    return Piece.AreaBomb(color);
                default:
                    throw new ArgumentException("Unknown special match type");
            }              
        }
    }
}