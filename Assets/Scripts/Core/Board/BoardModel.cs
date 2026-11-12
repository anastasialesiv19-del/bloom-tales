using System;
using System.Collections.Generic;
using System.Text;
using Core.Matching;

namespace Core.Board
{
    public class BoardModel
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Piece[,] _grid;
        private readonly ObstacleModel[,] _obstacles;

        public BoardModel(int height, int width)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("Attempt to enter incorrect board boundaries."); 
            }
            
            Width = width;
            Height = height;
            _grid = new Piece[height, width];
            _obstacles = new ObstacleModel[height, width];
        }

        public Piece Get(int row, int col)
        {
            return InBounds(row, col) ? _grid[row, col] : throw new IndexOutOfRangeException("Attempt to access a non-existent element.");
        }
        
        public void Set(int row, int col, Piece piece)
        {
            if (InBounds(row, col))
            {
                _grid[row, col] = piece;
            }
            else
            {
                throw new IndexOutOfRangeException("Attempt to write outside the board."); 
            }
        }

        public bool InBounds(int row, int col)
        {
            return row >= 0 && row < Height && col >= 0 && col < Width;
        }

        public void Swap(Pos a, Pos b)
        {
            if (InBounds(a.Row, a.Col) && InBounds(b.Row, b.Col))
            {
                (_grid[a.Row, a.Col], _grid[b.Row, b.Col]) = (_grid[b.Row, b.Col], _grid[a.Row, a.Col]); 
            }
            else
            {
                throw new IndexOutOfRangeException("Attempt to perform a swap with a non-existent element.");
            }
        }

        public IEnumerable<Pos> AllPositions()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    yield return new Pos(row, col);
                }
            }
        }

        public string ToDebugString()
        {
            var str = new StringBuilder();
            
            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    str.Append(PieceTypeMapping(_grid[row, col]));
                    str.Append(' ');
                }
                str.AppendLine();
            }
            return str.ToString();
        }
        
        public ObstacleModel GetObstacle(int row, int col)
        {
            return _obstacles[row, col];
        }

        public void SetObstacle(int row, int col, ObstacleModel obstacle)
        {
            _obstacles[row, col] = obstacle;
        }

        public bool HasObstacle(int row, int col)
        {
            return !GetObstacle(row, col).IsEmpty;
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
                    return piece.Color == null ? throw new ArgumentException("Piece kind requires color.") : PieceColorMapping(piece.Color.Value);
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
    }
}