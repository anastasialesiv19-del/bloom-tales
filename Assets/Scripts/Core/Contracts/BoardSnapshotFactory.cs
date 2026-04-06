using System;
using Core.Matching;

namespace Core.Contracts
{
    public static class BoardSnapshotFactory
    {
        public static BoardSnapshot Capture(BoardModel board)
        {
            if(board == null)
                throw new ArgumentNullException(nameof(board));
            
            var pieceStorage = new Piece [board.Height, board.Width];

            foreach (var pos in board.AllPositions())
            {
                var currentPiece = board.Get(pos.Row, pos.Col);
                pieceStorage[pos.Row, pos.Col] = currentPiece;
            }
            return new BoardSnapshot(board.Width, board.Height, pieceStorage);
        }
    }
}