using System;
using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Matching;
using Core.Randomness;


namespace Core.Generation
{
    public static class BoardGenerator
    {
        private static readonly PieceColor[] AllSpawnable =
            Enum.GetValues(typeof(PieceColor))
                .Cast<PieceColor>()
                .ToArray();
        
        public static BoardModel Generate(int height, int width, IRandomProvider provider = null)
        {
            var rng = provider ?? new SystemRandomProvider();
            for (var attempt = 0; attempt < 20; attempt++)
            {
                var board = new BoardModel(height,width);
                foreach (var piecePos in board.AllPositions())
                {
                    var piece = PieceForCell(piecePos.Row, piecePos.Col, board, rng);
                    board.Set(piecePos.Row, piecePos.Col, piece);
                }
                
                if (BoardShuffler.IsPlayableBoard(board)) return board;
            }
            throw new InvalidOperationException("Board generation failed");
        }

        private static Piece PieceForCell(int row, int col, BoardModel board, IRandomProvider rng)
        {
            List<PieceColor> forbiddenColors = new List<PieceColor>();
            
            if (row >= 2)
            {
                var pieceColor1 = board.Get(row - 1, col).Color;
                var pieceColor2 = board.Get(row - 2, col).Color;
                if (pieceColor1 != null && pieceColor2 != null){
                    var up1 = pieceColor1.Value;
                    var up2 = pieceColor2.Value;
                    
                    if (up1 == up2)
                    {
                        if (!forbiddenColors.Contains(up1))
                            forbiddenColors.Add(up2);
                    }
                }
            }

            if (col >= 2)
            {
                var pieceColor1 = board.Get(row, col - 1).Color;
                var pieceColor2 = board.Get(row, col - 2).Color;
                if (pieceColor1 != null && pieceColor2 != null){
                    var left1 = pieceColor1.Value;
                    var left2 = pieceColor2.Value;
                    
                    if (left1 == left2)
                    {
                        if(!forbiddenColors.Contains(left1))
                            forbiddenColors.Add(left1);
                    }
                }
            }

            var allowedColors = AllSpawnable.Where(color => !forbiddenColors.Contains(color)).ToList();
            var allowedIndex = allowedColors.Count > 0 ? rng.Next(allowedColors.Count) : throw new InvalidOperationException();
            return Piece.Normal(allowedColors[allowedIndex]);
        }
    }
}