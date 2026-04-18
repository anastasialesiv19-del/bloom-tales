using System;
using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public class BoardInputController
    {
        private Pos? selectedCell;

        public event Action<Move> MoveSubmitted;
        public event Action<Pos?> SelectionChanged;
        public Func<bool> CanAcceptInput { get; set; }

        public void OnCellClicked(int row, int col)
        {
            if (CanAcceptInput != null && !CanAcceptInput())
                return;
            
            var clicked = new Pos(row, col);
            if (!selectedCell.HasValue)
            {
                selectedCell = clicked;
                SelectionChanged?.Invoke(selectedCell);
                return;
            }

            if (selectedCell.Value.Equals(clicked))
            {
                selectedCell = null;
                SelectionChanged?.Invoke(null);
                return;
            }

            var move = new Move(selectedCell.Value, clicked);
            selectedCell = null;
            SelectionChanged?.Invoke(null);
            MoveSubmitted?.Invoke(move);
        }
        
        public void ClearSelection()
        {
            selectedCell = null;
            SelectionChanged?.Invoke(null);
        }
    }
}