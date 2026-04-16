using System;
using Core.Matching;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Unity
{
    public class BoardCellView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text pieceText;
        [SerializeField] private Image background; //or SpriteRenderer
        [SerializeField] private Image selectionHighlight;
        
        public int Row { get; private set; }
        public int Col { get; private set; }
        
        public event Action<int, int> Clicked;

        public void Initialize(int row, int col)
        {
            Row = row;
            Col = col;
            selectionHighlight.enabled = false;
        }

        public void SetPiece(Piece piece)
        {
            var viewData = PieceViewMapper.Map(piece);
            
            pieceText.text = viewData.Symbol;
            pieceText.color = Color.black;
            background.color = viewData.Color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Clicked cell [{Row},{Col}]");
            Clicked?.Invoke(Row, Col);
        }

        public void SetSelected(bool isSelected)
        {
            Debug.Log($"Cell [{Row},{Col}] selected = {isSelected}");
            selectionHighlight.enabled = isSelected;
        }
    }
}