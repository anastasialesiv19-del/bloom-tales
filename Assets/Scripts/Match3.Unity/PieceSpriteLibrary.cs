using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public class PieceSpriteLibrary : MonoBehaviour
    {
        [SerializeField] private Sprite redSprite;
        [SerializeField] private Sprite yellowSprite;
        [SerializeField] private Sprite greenSprite;
        [SerializeField] private Sprite cyanSprite;
        [SerializeField] private Sprite blueSprite;
        [SerializeField] private Sprite purpleSprite;
        
        [SerializeField] private Sprite redRowClearSprite;
        [SerializeField] private Sprite yellowRowClearSprite;
        [SerializeField] private Sprite greenRowClearSprite;
        [SerializeField] private Sprite cyanRowClearSprite;
        [SerializeField] private Sprite blueRowClearSprite;
        [SerializeField] private Sprite purpleRowClearSprite;

        [SerializeField] private Sprite redColumnClearSprite;
        [SerializeField] private Sprite yellowColumnClearSprite;
        [SerializeField] private Sprite greenColumnClearSprite;
        [SerializeField] private Sprite cyanColumnClearSprite;
        [SerializeField] private Sprite blueColumnClearSprite;
        [SerializeField] private Sprite purpleColumnClearSprite;

        [SerializeField] private Sprite redAreaBombSprite;
        [SerializeField] private Sprite yellowAreaBombSprite;
        [SerializeField] private Sprite greenAreaBombSprite;
        [SerializeField] private Sprite cyanAreaBombSprite;
        [SerializeField] private Sprite blueAreaBombSprite;
        [SerializeField] private Sprite purpleAreaBombSprite;
        
        [SerializeField] private Sprite colorBomb;

        public Sprite GetSprite(Piece piece)
        {
            if(piece.Kind == PieceKind.Empty) return null;
            if (piece.Kind == PieceKind.ColorBomb) return colorBomb;
            if (piece.Color == null) return null;
            return piece.Kind switch
            {
                PieceKind.Normal => GetNormalSprite(piece.Color.Value),
                PieceKind.RowClear => GetRowClearSprite(piece.Color.Value),
                PieceKind.ColumnClear => GetColumnClearSprite(piece.Color.Value),
                PieceKind.AreaBomb => GetAreaBombSprite(piece.Color.Value),
                _ => null
            };
        }
        
        private Sprite GetNormalSprite(PieceColor color)
        {
            return color switch
            {
                PieceColor.Red => redSprite,
                PieceColor.Yellow => yellowSprite,
                PieceColor.Green => greenSprite,
                PieceColor.Cyan => cyanSprite,
                PieceColor.Blue => blueSprite,
                PieceColor.Purple => purpleSprite,
                _ => null
            };
        }

        private Sprite GetRowClearSprite(PieceColor color)
        {
            return color switch
            {
                PieceColor.Red => redRowClearSprite,
                PieceColor.Yellow => yellowRowClearSprite,
                PieceColor.Green => greenRowClearSprite,
                PieceColor.Cyan => cyanRowClearSprite,
                PieceColor.Blue => blueRowClearSprite,
                PieceColor.Purple => purpleRowClearSprite,
                _ => null
            };
        }

        private Sprite GetColumnClearSprite(PieceColor color)
        {
            return color switch
            {
                PieceColor.Red => redColumnClearSprite,
                PieceColor.Yellow => yellowColumnClearSprite,
                PieceColor.Green => greenColumnClearSprite,
                PieceColor.Cyan => cyanColumnClearSprite,
                PieceColor.Blue => blueColumnClearSprite,
                PieceColor.Purple => purpleColumnClearSprite,
                _ => null
            };
        }

        private Sprite GetAreaBombSprite(PieceColor color)
        {
            return color switch
            {
                PieceColor.Red => redAreaBombSprite,
                PieceColor.Yellow => yellowAreaBombSprite,
                PieceColor.Green => greenAreaBombSprite,
                PieceColor.Cyan => cyanAreaBombSprite,
                PieceColor.Blue => blueAreaBombSprite,
                PieceColor.Purple => purpleAreaBombSprite,
                _ => null
            };
        }
    }
}