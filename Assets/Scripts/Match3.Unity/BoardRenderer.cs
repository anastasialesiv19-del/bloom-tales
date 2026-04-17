using System;
using Core.Contracts;
using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private BoardCellView cellPrefab;
        [SerializeField] private Transform gridRoot;
        [SerializeField] private PieceSpriteLibrary pieceSpriteLibrary;
        private BoardCellView[,] cells;
        private int _height;
        private int _width;
        public event Action<int, int> CellClicked;

        public void BuildGrid(int height, int width)
        {
            _height = height;
            _width = width;
            cells = new BoardCellView[height, width];
            
            for(var row = 0; row < height; row++)
            for (var col = 0; col < width; col++)
            {
                var cell = Instantiate(cellPrefab, gridRoot);
                cell.Initialize(row, col, pieceSpriteLibrary);
                cells[row, col] = cell;
                cell.Clicked += HandleCellClicked;
            }
        }
        
        public void Render(BoardSnapshot snapshot)
        {
            if (cells == null || snapshot.Height != _height || snapshot.Width != _width)
                BuildGrid(snapshot.Height, snapshot.Width);
            
            for (var row = 0; row < _height; row++)
            for (var col = 0; col < _width; col++)
                cells[row, col].SetPiece(snapshot.Get(row, col));
            
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
                    cells[row, col].SetSelected(true);
                else
                    cells[row, col].SetSelected(false);
            }
            
        }

        public void SetHint(Move? move)
        {
            ClearHintInternal();

            if (!move.HasValue) return;
            
            var from = move.Value.From;
            var to = move.Value.To;
            cells[from.Row, from.Col].SetHint(true);
            cells[to.Row, to.Col].SetHint(true);
        }

        private void ClearHintInternal()
        {
            for (var row = 0; row < _height; row++)
            for (var col = 0; col < _width; col++)
            {
                cells[row, col].SetHint(false);
            }
        }
    }
}