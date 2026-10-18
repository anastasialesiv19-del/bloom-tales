using System;
using Core.Board;
using Core.Matching;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Unity
{
    public class BoardCellView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image pieceImage;
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private Image hintHighlight;
        [SerializeField] private Image specialOverlay;
        [SerializeField] private Image obstacleImage;
        [SerializeField] private Sprite boxObstacleSprite;
        private PieceSpriteLibrary spriteLibrary;
        private bool isSelected;
        private bool isHinted;
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
            RefreshHighlights();
        }

        public void SetPiece(Piece piece)
        {
            var viewData = PieceViewMapper.Map(piece, spriteLibrary);
            
            pieceImage.sprite = viewData.Sprite;
            pieceImage.enabled = viewData.Sprite != null;
            
            specialOverlay.sprite = viewData.OverlaySprite;
            specialOverlay.enabled = viewData.Sprite != null && viewData.OverlaySprite != null;
        }
        
        public void SetObstacle(ObstacleModel obstacle)
        {
            if (obstacleImage == null)
                return;

            if (obstacle.IsEmpty)
            {
                obstacleImage.gameObject.SetActive(false);
                obstacleImage.sprite = null;
                return;
            }

            obstacleImage.gameObject.SetActive(true);
            pieceImage.enabled = false;
            pieceImage.sprite = null;
            specialOverlay.enabled = false;
            specialOverlay.sprite = null;
            
            switch (obstacle.Kind)
            {
                case ObstacleKind.Box:
                    obstacleImage.sprite = boxObstacleSprite;
                    break;

                default:
                    obstacleImage.sprite = null;
                    obstacleImage.gameObject.SetActive(false);
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(Row, Col);
        }

        public void SetSelected(bool isSelected)
        {
            this.isSelected = isSelected;
            RefreshHighlights();
        }

        public void SetHint(bool isHinted)
        {
            this.isHinted = isHinted;
            RefreshHighlights();
        }

        private void RefreshHighlights()
        {
            hintHighlight.enabled = isHinted;
            selectionHighlight.enabled = isSelected && !isHinted;
        }
    }
}