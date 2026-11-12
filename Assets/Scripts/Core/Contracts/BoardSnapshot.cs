using System;
using System.Text;
using Core.Board;

namespace Core.Contracts
{
    public class BoardSnapshot
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Piece[,] _pieceStorage;
        private readonly ObstacleModel[,] _obstacleStorage;

        public BoardSnapshot(int width, int height, Piece[,] pieceStorage,  ObstacleModel[,] obstacleStorage)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Attempt to enter incorrect board snapshot size.");
            }
            if (pieceStorage == null)
            {
                throw new ArgumentNullException(nameof(pieceStorage));
            }
            if (pieceStorage.GetLength(0) != height || pieceStorage.GetLength(1) != width)
            {
                throw new InvalidOperationException("Error writing data from the board.");
            }
            if (obstacleStorage == null)
            {
                throw new ArgumentNullException(nameof(obstacleStorage));
            }
            if (obstacleStorage.GetLength(0) != height || obstacleStorage.GetLength(1) != width)
            {
                throw new InvalidOperationException("Error writing obstacle data from the board.");
            }

            Width = width;
            Height = height;
            _pieceStorage = (Piece[,])pieceStorage.Clone();
            _obstacleStorage = (ObstacleModel[,])obstacleStorage.Clone();
        }

        public Piece Get(int row, int col)
        {
            if (row >= 0 && row < Height && col >= 0 && col < Width)
            {
                return _pieceStorage[row, col];
            }
            throw new IndexOutOfRangeException("Attempt to access a non-existent element.");
        }
        
        public ObstacleModel GetObstacle(int row, int col)
        {
            if (row >= 0 && row < Height && col >= 0 && col < Width)
            {
                return _obstacleStorage[row, col];
            }
            throw new IndexOutOfRangeException("Attempt to access a non-existent obstacle element.");
        }

        public bool HasObstacle(int row, int col)
        {
            return !GetObstacle(row, col).IsEmpty;
        }

        public string ToDebugString()
        {
            var str = new StringBuilder();
            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    str.Append(HasObstacle(row, col) ? ObstacleTypeMapping(_obstacleStorage[row, col]) : PieceTypeMapping(_pieceStorage[row, col]));
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
                    if (piece.Color == null)
                    {
                        throw new ArgumentException("Piece kind requires color.");
                    }
                    var color = PieceColorMapping(piece.Color.Value);
                    return "H" + color;
                case PieceKind.ColumnClear:
                    if (piece.Color == null)
                    {
                        throw new ArgumentException("Piece kind requires color.");
                    }
                    color = PieceColorMapping(piece.Color.Value);
                    return "V" + color;
                case PieceKind.AreaBomb:
                    if (piece.Color == null)
                    {
                        throw new ArgumentException("Piece kind requires color.");
                    }
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
            return color switch
            {
                PieceColor.Red => "R",
                PieceColor.Yellow => "Y",
                PieceColor.Green => "G",
                PieceColor.Cyan => "C",
                PieceColor.Blue => "B",
                PieceColor.Purple => "P",
                _ => throw new InvalidOperationException()
            };
        }
        
        private static string ObstacleTypeMapping(ObstacleModel obstacle)
        {
            return obstacle.Kind switch
            {
                ObstacleKind.None => "_",
                ObstacleKind.Box => "BX",
                _ => throw new InvalidOperationException()
            };
        }
    }
}