using Core.Board;
using UnityEngine;

namespace Match3.Unity.Board
{
    public class PieceSpriteLibrary : MonoBehaviour
    {
        [SerializeField] private Sprite redSprite;
        [SerializeField] private Sprite yellowSprite;
        [SerializeField] private Sprite greenSprite;
        [SerializeField] private Sprite cyanSprite;
        [SerializeField] private Sprite blueSprite;
        [SerializeField] private Sprite purpleSprite;
        
        [SerializeField] private Sprite rowClearOverlay;
        [SerializeField] private Sprite columnClearOverlay;
        [SerializeField] private Sprite areaBombOverlay;
        [SerializeField] private Sprite colorBomb;

        public Sprite GetSprite(Piece piece)
        {
            if (piece.Kind == PieceKind.Empty)
            {
                return null;
            }
            if (piece.Kind == PieceKind.ColorBomb)
            {
                return colorBomb;
            }
            if (piece.Color == null)
            {
                return null;
            }
            
            return piece.Kind switch
            {
                PieceKind.Normal => GetNormalSprite(piece.Color.Value),
                PieceKind.RowClear => GetNormalSprite(piece.Color.Value),
                PieceKind.ColumnClear => GetNormalSprite(piece.Color.Value),
                PieceKind.AreaBomb => GetNormalSprite(piece.Color.Value),
                _ => null
            };
        }

        public Sprite GetOverlaySprite(PieceKind kind)
        {
            return kind switch
            {
                PieceKind.RowClear => rowClearOverlay,
                PieceKind.ColumnClear => columnClearOverlay,
                PieceKind.AreaBomb => areaBombOverlay,
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
    }
}