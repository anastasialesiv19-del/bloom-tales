using Core.Contracts;
using UnityEngine;

namespace Match3.Unity
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private BoardCellView cellPrefab;
        [SerializeField] private Transform gridRoot;
        private BoardCellView[,] cells;
        private int _height;
        private int _width;

        public void BuildGrid(int height, int width)
        {
            _height = height;
            _width = width;
            cells = new BoardCellView[height, width];
            
            for(var row = 0; row < height; row++)
            for (var col = 0; col < width; col++)
            {
                var cell = Instantiate(cellPrefab, gridRoot);
                cell.Initialize(row, col);
                cells[row, col] = cell;
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
    }
}