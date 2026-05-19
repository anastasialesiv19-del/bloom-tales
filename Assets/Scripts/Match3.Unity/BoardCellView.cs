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
        [SerializeField] private Image obstacleImage;
        [SerializeField] private Sprite boxObstacleSprite;
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

        public void SetPiece(Piece piece)
        {
            // piece rendering залишається як є
            // obstacle окремо ставиться через SetObstacle(...)
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
            selectionHighlight.enabled = isSelected;
        }

        public void SetHint(bool isHinted)
        {
            hintHighlight.enabled = isHinted;
        }
    }
}