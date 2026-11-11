using System;
using Core.Board;
using Core.Contracts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Match3.Unity
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private BoardCellView cellPrefab;
        [SerializeField] private Transform gridRoot;
        [SerializeField] private PieceSpriteLibrary pieceSpriteLibrary;

        public event Action<int, int> CellClicked;
        private BoardCellView[,] _cells;
        private BoardSnapshot _currentSnapshot;
        private int _height;
        private int _width;


        public void BuildGrid(int height, int width)
        {
            _height = height;
            _width = width;
            _cells = new BoardCellView[height, width];

            for (var row = 0; row < height; row++)
            for (var col = 0; col < width; col++)
            {
                var cell = Instantiate(cellPrefab, gridRoot);
                cell.Initialize(row, col, pieceSpriteLibrary);
                _cells[row, col] = cell;
                cell.Clicked += HandleCellClicked;
            }
        }

        public void Render(BoardSnapshot snapshot)
        {
            if (_cells == null || snapshot.Height != _height || snapshot.Width != _width)
                BuildGrid(snapshot.Height, snapshot.Width);

            for (var row = 0; row < _height; row++)
            for (var col = 0; col < _width; col++)
            {
                var piece = snapshot.Get(row, col);
                var obstacle = snapshot.GetObstacle(row, col);

                _cells[row, col].SetPiece(piece);
                _cells[row, col].SetObstacle(obstacle);
            }

            _currentSnapshot = snapshot;
        }

        public IEnumerator RenderSwap(BoardSnapshot snapshot, Move move, float duration)
        {
            if (_currentSnapshot == null || _cells == null || snapshot.Height != _height || snapshot.Width != _width || duration <= 0f)
            {
                Render(snapshot);
                yield break;
            }

            var fromCell = _cells[move.From.Row, move.From.Col];
            var toCell = _cells[move.To.Row, move.To.Col];

            fromCell.ResetPiecePosition();
            toCell.ResetPiecePosition();
            fromCell.SetPieceOpacity(1f);
            toCell.SetPieceOpacity(1f);

            var offset = toCell.PieceWorldPosition - fromCell.PieceWorldPosition;
            var elapsed = 0f;

            try
            {
                while (elapsed < duration)
                {
                    var progress = Mathf.Clamp01(elapsed / duration);
                    var easedProgress = Mathf.SmoothStep(0f, 1f, progress);

                    fromCell.SetPieceWorldOffset(offset * easedProgress);
                    toCell.SetPieceWorldOffset(-offset * easedProgress);

                    yield return null;
                    elapsed += Time.deltaTime;
                }
                fromCell.SetPieceWorldOffset(offset);
                toCell.SetPieceWorldOffset(-offset);
            }
            finally
            {
                fromCell.ResetPiecePosition();
                toCell.ResetPiecePosition();
            }
            Render(snapshot);
        }
        
        public IEnumerator RenderGravity(BoardSnapshot snapshot, IReadOnlyList<PieceFall> falls, float duration)
        {
            if (_currentSnapshot == null || _cells == null || snapshot.Height != _height || snapshot.Width != _width || falls == null || falls.Count == 0 || duration <= 0f)
            {
                Render(snapshot);
                yield break;
            }

            var movingCells = new List<BoardCellView>();
            var offsets = new List<Vector3>();

            // Спочатку відновлюємо позиції всіх зображень.
            foreach (var fall in falls)
            {
                _cells[fall.From.Row, fall.From.Col].ResetPiecePosition();
                _cells[fall.To.Row, fall.To.Col].ResetPiecePosition();
            }

            foreach (var fall in falls)
            {
                var source = _cells[fall.From.Row, fall.From.Col];
                var target = _cells[fall.To.Row, fall.To.Col];

                source.SetPieceOpacity(1f);

                movingCells.Add(source);
                offsets.Add(target.PieceWorldPosition - source.PieceWorldPosition);
            }

            float elapsed = 0f;

            try
            {
                while (elapsed < duration)
                {
                    float progress = Mathf.Clamp01(elapsed / duration);
                    float easedProgress = progress * progress;

                    for (int i = 0; i < movingCells.Count; i++)
                        movingCells[i].SetPieceWorldOffset(
                            offsets[i] * easedProgress);

                    yield return null;
                    elapsed += Time.deltaTime;
                }

                for (int i = 0; i < movingCells.Count; i++)
                    movingCells[i].SetPieceWorldOffset(offsets[i]);
            }
            finally
            {
                foreach (var cell in movingCells)
                    cell.ResetPiecePosition();
            }

            Render(snapshot);
        }

        public void RebuildAndRender(BoardSnapshot snapshot)
        {
            ClearGrid();
            _cells = null;
            _height = 0;
            _width = 0;
            Render(snapshot);
        }

        private void ClearGrid()
        {
            if (gridRoot == null)
                return;

            for (int i = gridRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(gridRoot.GetChild(i).gameObject);
            }
        }

        private void HandleCellClicked(int row, int col)
        {
            CellClicked?.Invoke(row, col);
        }

        public void SetSelection(Pos? selected)
        {
            for (var row = 0; row < _height; row++)
            for (var col = 0; col < _width; col++)
            {
                if (selected.HasValue && selected.Value.Row == row && selected.Value.Col == col)
                    _cells[row, col].SetSelected(true);
                else
                    _cells[row, col].SetSelected(false);
            }
        }

        public void SetHint(Move? move)
        {
            ClearHintInternal();

            if (!move.HasValue) return;

            var from = move.Value.From;
            var to = move.Value.To;
            _cells[from.Row, from.Col].SetHint(true);
            _cells[to.Row, to.Col].SetHint(true);
        }

        public IEnumerator RenderAnimated(BoardSnapshot snapshot, float duration)
        {
            if (_currentSnapshot == null || _cells == null || snapshot.Height != _height || snapshot.Width != _width ||
                duration <= 0f)
            {
                Render(snapshot);
                yield break;
            }

            var changedCells = new List<BoardCellView>();

            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    bool pieceChanged =
                        !_currentSnapshot.Get(row, col).Equals(snapshot.Get(row, col));

                    bool obstaclePresenceChanged =
                        _currentSnapshot.GetObstacle(row, col).IsEmpty !=
                        snapshot.GetObstacle(row, col).IsEmpty;

                    if (pieceChanged || obstaclePresenceChanged)
                        changedCells.Add(_cells[row, col]);
                }
            }

            if (changedCells.Count == 0)
            {
                Render(snapshot);
                yield break;
            }

            yield return FadePieces(changedCells, 1f, 0f, duration);

            Render(snapshot);

            foreach (var cell in changedCells)
            {
                cell.SetPieceOpacity(0f);
                cell.SetObstacleOpacity(0f);
            }

            yield return FadePieces(changedCells, 0f, 1f, duration);
        }

        private void ClearHintInternal()
        {
            for (var row = 0; row < _height; row++)
            for (var col = 0; col < _width; col++)
            {
                _cells[row, col].SetHint(false);
            }
        }

        private IEnumerator FadePieces(List<BoardCellView> animatedCells, float from, float to, float duration)
        {
            var elapsed = 0f;

            while (elapsed < duration)
            {
                var progress = Mathf.Clamp01(elapsed / duration);
                var opacity = Mathf.Lerp(from, to, progress);

                foreach (var cell in animatedCells)
                {
                    cell.SetPieceOpacity(opacity);
                    cell.SetObstacleOpacity(opacity);
                }

                yield return null;
                elapsed += Time.deltaTime;
            }

            foreach (var cell in animatedCells)
            {
                cell.SetPieceOpacity(to);
                cell.SetObstacleOpacity(to);
            }
        }
    }
}