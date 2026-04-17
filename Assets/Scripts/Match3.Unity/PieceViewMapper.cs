using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public static class PieceViewMapper
    {
        public static PieceViewData Map(Piece piece, PieceSpriteLibrary spriteLibrary)
        {
            var sprite = spriteLibrary.GetSprite(piece);
            var symbol = KindSymbol(piece);
            return new PieceViewData(symbol, sprite);
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
    }

    public readonly struct PieceViewData
    {
        public string Symbol { get; }
        public Sprite Sprite { get; }

        public PieceViewData(string symbol, Sprite sprite)
        {
            Symbol = symbol;
            Sprite = sprite;
        }
    }
}