using System;
using System.Collections.Generic;
using System.Text;
using Core.Board;
using Core.Matching;

namespace Core.Contracts
{
    public class BoardSnapshot
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Piece[,] pieceStorage;
        private readonly ObstacleModel[,] obstacleStorage;

        public BoardSnapshot(int width, int height, Piece[,] pieceStorage,  ObstacleModel[,] obstacleStorage)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Attempt to enter incorrect board snapshot size.");
            if (pieceStorage == null)
                throw new ArgumentNullException(nameof(pieceStorage));
            if (pieceStorage.GetLength(0) != height ||
                pieceStorage.GetLength(1) != width)
                throw new InvalidOperationException("Error writing data from the board.");
            if (obstacleStorage == null)
                throw new ArgumentNullException(nameof(obstacleStorage));
            if (obstacleStorage.GetLength(0) != height ||
                obstacleStorage.GetLength(1) != width)
                throw new InvalidOperationException("Error writing obstacle data from the board.");

            Width = width;
            Height = height;
            this.pieceStorage = (Piece[,])pieceStorage.Clone();
            this.obstacleStorage = (ObstacleModel[,])obstacleStorage.Clone();
        }

        public Piece Get(int row, int col)
        {
            if (row >= 0 && row < Height && col >= 0 && col < Width)
                return pieceStorage[row, col];
            throw new IndexOutOfRangeException("Attempt to access a non-existent element.");
        }
        
        public ObstacleModel GetObstacle(int row, int col)
        {
            if (row >= 0 && row < Height && col >= 0 && col < Width)
                return obstacleStorage[row, col];

            throw new IndexOutOfRangeException("Attempt to access a non-existent obstacle element.");
        }

        public bool HasObstacle(int row, int col)
        {
            return !GetObstacle(row, col).IsEmpty;
        }

        public string ToDebugString()
        {
            StringBuilder str = new StringBuilder();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (HasObstacle(row, col))
                        str.Append(ObstacleTypeMapping(obstacleStorage[row, col]));
                    else
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
                case PieceColor.Red:
                    return "R";
                case PieceColor.Yellow:
                    return "Y";
                case PieceColor.Green:
                    return "G";
                case PieceColor.Cyan:
                    return "C";
                case PieceColor.Blue:
                    return "B";
                case PieceColor.Purple:
                    return "P";
                default:
                    throw new InvalidOperationException();
            }
        }
        
        private static string ObstacleTypeMapping(ObstacleModel obstacle)
        {
            switch (obstacle.Kind)
            {
                case ObstacleKind.None:
                    return "_";
                case ObstacleKind.Box:
                    return "BX";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}