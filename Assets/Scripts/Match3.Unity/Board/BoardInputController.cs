using System;
using Core.Board;

namespace Match3.Unity.Board
{
    public class BoardInputController
    {
        public Func<bool> CanAcceptInput { get; set; }
        public event Action<Move> MoveSubmitted;
        public event Action<Pos?> SelectionChanged;
        private Pos? _selectedCell;

        public void OnCellClicked(int row, int col)
        {
            if (CanAcceptInput != null && !CanAcceptInput())
            {
                return;
            }
            
            var clicked = new Pos(row, col);
            if (!_selectedCell.HasValue)
            {
                _selectedCell = clicked;
                SelectionChanged?.Invoke(_selectedCell);
                return;
            }

            if (_selectedCell.Value.Equals(clicked))
            {
                _selectedCell = null;
                SelectionChanged?.Invoke(null);
                return;
            }
            
            var dx = Math.Abs(clicked.Row - _selectedCell.Value.Row);
            var dy = Math.Abs(clicked.Col - _selectedCell.Value.Col);
            if (dx + dy != 1)
            {
                _selectedCell = clicked;
                SelectionChanged?.Invoke(_selectedCell);
                return;
            }

            var move = new Move(_selectedCell.Value, clicked);
            _selectedCell = null;
            SelectionChanged?.Invoke(null);
            MoveSubmitted?.Invoke(move);
        }
        
        public void ClearSelection()
        {
            _selectedCell = null;
            SelectionChanged?.Invoke(null);
        }
    }
}