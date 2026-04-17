using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public class PieceSpriteLibrary : MonoBehaviour
    {
        [SerializeField] private Sprite redSprite;
        [SerializeField] private Sprite blueSprite;
        [SerializeField] private Sprite greenSprite;
        [SerializeField] private Sprite yellowSprite;
        [SerializeField] private Sprite colorBomb;

        public Sprite GetSprite(Piece piece)
        {
            if(piece.Kind == PieceKind.Empty) return null;
            if (piece.Kind == PieceKind.ColorBomb) return colorBomb;
            if (piece.Color == null) return null;
            return piece.Color.Value switch
            {
                PieceColor.Red => redSprite,
                PieceColor.Blue => blueSprite,
                PieceColor.Green => greenSprite,
                PieceColor.Yellow => yellowSprite,
                _ => null
            };
        }
    }
}