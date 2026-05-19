using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public static class PieceViewMapper
    {
        public static PieceViewData Map(Piece piece, PieceSpriteLibrary spriteLibrary)
        {
            var sprite = spriteLibrary.GetSprite(piece);
            return new PieceViewData(sprite);
        }
    }

    public readonly struct PieceViewData
    {
        public Sprite Sprite { get; }

        public PieceViewData(Sprite sprite)
        {
            Sprite = sprite;
        }
    }
}