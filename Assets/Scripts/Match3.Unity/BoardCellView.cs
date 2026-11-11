using System;
using Core.Board;
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
        
        private PieceSpriteLibrary _spriteLibrary;
        private Vector3 _initialPieceScale;
        private Vector3 _initialOverlayScale;
        private Vector3 _initialObstacleScale;
        private Vector3 _initialPiecePosition;
        private Vector3 _initialOverlayPosition;
        private bool _isSelected;
        private bool _isHinted;
        
        public int Row { get; private set; }
        public int Col { get; private set; }
        public Vector3 PieceWorldPosition => pieceImage.rectTransform.position;
        public event Action<int, int> Clicked;

        public void Initialize(int row, int col, PieceSpriteLibrary spriteLibrary)
        {
            Row = row;
            Col = col;
            _spriteLibrary = spriteLibrary;
            _initialPieceScale = pieceImage.rectTransform.localScale;
            _initialOverlayScale = specialOverlay.rectTransform.localScale;
            _initialPiecePosition = pieceImage.rectTransform.localPosition;
            _initialOverlayPosition = specialOverlay.rectTransform.localPosition;
            if (obstacleImage != null)
            {
                _initialObstacleScale = obstacleImage.rectTransform.localScale;
            }

            selectionHighlight.enabled = false;
            hintHighlight.enabled = false;
            RefreshHighlights();
        }

        public void SetPiece(Piece piece)
        {
            ResetPiecePosition();
            SetPieceOpacity(1f);

            var viewData = PieceViewMapper.Map(piece, _spriteLibrary);

            pieceImage.sprite = viewData.Sprite;
            pieceImage.enabled = viewData.Sprite != null;

            specialOverlay.sprite = viewData.OverlaySprite;
            specialOverlay.enabled = viewData.Sprite != null && viewData.OverlaySprite != null;
        }

        public void SetObstacle(ObstacleModel obstacle)
        {
            if (obstacleImage == null)
                return;

            SetObstacleOpacity(1f);

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
            _isSelected = isSelected;
            RefreshHighlights();
        }

        public void SetHint(bool isHinted)
        {
            _isHinted = isHinted;
            RefreshHighlights();
        }

        public void SetPieceOpacity(float opacity)
        {
            opacity = Mathf.Clamp01(opacity);

            SetImageOpacity(pieceImage, opacity);
            SetImageOpacity(specialOverlay, opacity);

            var scale = Mathf.Lerp(0.75f, 1f, opacity);

            pieceImage.rectTransform.localScale = _initialPieceScale * scale;
            specialOverlay.rectTransform.localScale = _initialOverlayScale * scale;
        }

        public void SetObstacleOpacity(float opacity)
        {
            if (obstacleImage == null)
            {
                return;
            }

            opacity = Mathf.Clamp01(opacity);
            SetImageOpacity(obstacleImage, opacity);

            float scale = Mathf.Lerp(0.75f, 1f, opacity);
            obstacleImage.rectTransform.localScale = _initialObstacleScale * scale;
        }
        
        public void SetPieceWorldOffset(Vector3 offset)
        {
            var pieceTransform = pieceImage.rectTransform;
            var overlayTransform = specialOverlay.rectTransform;

            pieceTransform.position =
                pieceTransform.parent.TransformPoint(_initialPiecePosition) + offset;

            overlayTransform.position =
                overlayTransform.parent.TransformPoint(_initialOverlayPosition) + offset;
        }

        public void ResetPiecePosition()
        {
            pieceImage.rectTransform.localPosition = _initialPiecePosition;
            specialOverlay.rectTransform.localPosition = _initialOverlayPosition;
        }

        private static void SetImageOpacity(Image image, float opacity)
        {
            if (image == null)
            {
                return;
            }

            var color = image.color;
            color.a = opacity;
            image.color = color;
        }

        private void RefreshHighlights()
        {
            hintHighlight.enabled = _isHinted;
            selectionHighlight.enabled = _isSelected && !_isHinted;
        }
    }
}