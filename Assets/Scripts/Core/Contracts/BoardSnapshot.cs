using System;
using System.Collections.Generic;
using System.Text;
using Core.Matching;

namespace Core.Contracts
{
    public class BoardSnapshot
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Piece[,] pieceStorage;

        public BoardSnapshot(int width, int height, Piece[,] pieceStorage)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Attempt to enter incorrect board snapshot size.");
            if (pieceStorage == null)
                throw new ArgumentNullException(nameof(pieceStorage));
            if (pieceStorage.GetLength(0) != height ||
                pieceStorage.GetLength(1) != width)
                throw new InvalidOperationException("Error writing data from the board.");

            Width = width;
            Height = height;
            this.pieceStorage = (Piece[,])pieceStorage.Clone();
        }

        public Piece Get(int row, int col)
        {
            if (row >= 0 && row < Height && col >= 0 && col < Width)
                return pieceStorage[row, col];
            throw new IndexOutOfRangeException("Attempt to access a non-existent element.");
        }

        public string ToDebugString()
        {
            StringBuilder str = new StringBuilder();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    str.Append(PieceTypeMapping(pieceStorage[row, col]));
                    str.Append(' ');
                }

                str.AppendLine();
            }

            return str.ToString();
        }
        
        private static string PieceTypeMapping(Piece piece)
        {
            switch (piece.Kind)
            {
                case PieceKind.Empty:
                    return "_";
                case PieceKind.ColorBomb:
                    return "CB";
                case PieceKind.RowClear:
                    if (piece.Color == null) throw new ArgumentException("Piece kind requires color.");
                    var color = PieceColorMapping(piece.Color.Value);
                    return "H" + color;
                case PieceKind.ColumnClear:
                    if (piece.Color == null) throw new ArgumentException("Piece kind requires color.");
                    color = PieceColorMapping(piece.Color.Value);
                    return "V" + color;
                case PieceKind.AreaBomb:
                    if (piece.Color == null) throw new ArgumentException("Piece kind requires color.");
                    color = PieceColorMapping(piece.Color.Value);
                    return "A" + color;
                case PieceKind.Normal:
                    return piece.Color == null ? throw new ArgumentException("Piece kind requires color.") : 
                        PieceColorMapping(piece.Color.Value);
                default:
                    throw new InvalidOperationException();
            }
            
        }

        private static string PieceColorMapping(PieceColor color)
        {
            switch (color)
            {
                case PieceColor.Blue:
                    return "B";
                case PieceColor.Green:
                    return "G";
                case PieceColor.Red:
                    return "R";
                case PieceColor.Yellow:
                    return "Y";
                default:
                    throw new InvalidOperationException();
            }
        }

    }
}