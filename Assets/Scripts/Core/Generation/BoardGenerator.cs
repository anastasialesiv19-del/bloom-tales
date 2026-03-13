using System;
using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Match;


namespace Core.Generation
{
    public static class BoardGenerator
    {
        static Random _random = new Random();
        private static readonly PieceType[] AllSpawnable =
            Enum.GetValues(typeof(PieceType))
                .Cast<PieceType>()
                .Where(t => t != PieceType.Empty)
                .ToArray();
        
        public static BoardModel Generate(int height, int width)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                var board = new BoardModel(height,width);
                foreach (var piecePos in board.AllPositions())
                {
                    var pieceType = PickTypeForCell(piecePos.Row, piecePos.Col, board);
                    board.Set(piecePos.Row, piecePos.Col, pieceType);
                }
                
                if (MatchFinder.FindMatches(board).Count == 0) return board;
            }

            throw new InvalidOperationException("Board generation failed");
        }

        private static PieceType PickTypeForCell(int row, int col, BoardModel board)
        {
            List<PieceType> allowedTypes = new List<PieceType>();
            List<PieceType> forbiddenTypes = new List<PieceType>();
            PieceType left1 = 0, left2 = 0;
            PieceType up1 = 0, up2 = 0;
            
            if (row >= 2)
            {
                left1 = board.Get(row - 1, col); left2 = board.Get(row - 2, col);
                if (left1 == left2 && left1 != PieceType.Empty)
                {
                    if(!forbiddenTypes.Contains(left1))
                        forbiddenTypes.Add(left1);
                }
            }

            if (col >= 2)
            {
                up1 = board.Get(row, col - 1); up2 = board.Get(row, col - 2);
                if (up1 == up2 && up1 != PieceType.Empty)
                {
                    if(!forbiddenTypes.Contains(up1))
                        forbiddenTypes.Add(up1);
                }

            }
            
            foreach (PieceType type in AllSpawnable)
            {
                if (!forbiddenTypes.Contains(type))
                {
                    allowedTypes.Add(type);
                }
            }
            var allowedIndex = allowedTypes.Count > 0 ? _random.Next(allowedTypes.Count) : throw new InvalidOperationException();
            return allowedTypes[allowedIndex];
        }
    }
}