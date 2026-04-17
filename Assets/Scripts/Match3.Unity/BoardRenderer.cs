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

        public void SetSelection(Pos? selected)
        {
            for (var row = 0; row < cells.GetLength(0); row++)
            for (var col = 0; col < cells.GetLength(1); col++)
            {
                if (selected.HasValue && selected.Value.Row == row && selected.Value.Col == col)
                    cells[row, col].SetSelected(true);
                else
                    cells[row, col].SetSelected(false);
            }
            
        }
        
        private void HandleCellClicked(int row, int col)
        {
            CellClicked?.Invoke(row, col);
        }
    }
}