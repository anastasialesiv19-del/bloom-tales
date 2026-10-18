using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public static class PieceViewMapper
    {
        public static PieceViewData Map(Piece piece, PieceSpriteLibrary spriteLibrary)
        {
            var sprite = spriteLibrary.GetSprite(piece);
            var overlaySprite = spriteLibrary.GetOverlaySprite(piece.Kind);
            return new PieceViewData(sprite, overlaySprite);
        }
    }

    public readonly struct PieceViewData
    {
        public Sprite Sprite { get; }
        public Sprite OverlaySprite { get;}

        public PieceViewData(Sprite sprite, Sprite overlaySprite)
        {
            Sprite = sprite;
            OverlaySprite = overlaySprite;
        }
    }
}