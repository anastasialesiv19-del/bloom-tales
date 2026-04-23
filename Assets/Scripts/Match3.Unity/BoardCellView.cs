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
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text pieceText;
        [SerializeField] private Image pieceImage; //or SpriteRenderer
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private Image hintHighlight;
        private PieceSpriteLibrary spriteLibrary;
        public int Row { get; private set; }
        public int Col { get; private set; }
        
        public event Action<int, int> Clicked;

        public void Initialize(int row, int col, PieceSpriteLibrary spriteLibrary)
        {
            Row = row;
            Col = col;
            this.spriteLibrary = spriteLibrary;
            selectionHighlight.enabled = false;
            hintHighlight.enabled = false;
        }

        public void SetBackground(Sprite sprite)
        {
            if (backgroundImage == null)
                return;
            backgroundImage.sprite = sprite;
            backgroundImage.enabled = sprite != null;
        }

        public void SetPiece(Piece piece)
        {
            var viewData = PieceViewMapper.Map(piece, spriteLibrary);

            if (viewData.Sprite != null)
            {
                pieceImage.enabled = true;
                pieceImage.sprite = viewData.Sprite;
            }
            else
            {
                pieceImage.enabled = false;
                pieceImage.sprite = null;
            }

            if (string.IsNullOrEmpty(viewData.Symbol))
            {
                pieceText.text = "";
                pieceText.enabled = false;
            }
            else
            {
                pieceText.text = viewData.Symbol;
                pieceText.enabled = true;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(Row, Col);
        }

        public void SetSelected(bool isSelected)
        {
            selectionHighlight.enabled = isSelected;
        }

        public void SetHint(bool isHinted)
        {
            hintHighlight.enabled = isHinted;
        }
    }
}