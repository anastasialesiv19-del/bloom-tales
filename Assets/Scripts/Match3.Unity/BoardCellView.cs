using Core.Matching;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Unity
{
    public class BoardCellView : MonoBehaviour
    {
        [SerializeField] private TMP_Text pieceText;
        [SerializeField] private Image background; //or SpriteRenderer
        
        public int Row { get; private set; }
        public int Col { get; private set; }

        public void Initialize(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public void SetPiece(Piece piece)
        {
            var viewData = PieceViewMapper.Map(piece);
            
            pieceText.text = viewData.Symbol;
            pieceText.color = Color.black;
            background.color = viewData.Color;
            
        }
    }
}