using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Matching
{
    public class BoardModel
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Piece[,] grid;

        public BoardModel(int height, int width)
        {
            if (width <= 0 || height <= 0)
            {
                throw new  ArgumentOutOfRangeException("Спроба введення некоректних меж дошки.");

            }
            Width = width;
            Height = height;
            grid = new Piece[height, width];
        }

        public Piece Get(int row, int col)
        {
            if (InBounds(row, col))
            {
                return grid[row, col];
            }
            else{
                throw new IndexOutOfRangeException("Спроба доступу до не існуючого елемента.");
            }
            
        }

        public bool TryGet(int row, int col, out Piece piece)
        {
            if (InBounds(row, col))
            {
                piece = grid[row, col];
                return true;
            }
            else
            {
                 piece = Piece.Empty();
                 return false;
            }
        }

        public bool TrySet(int row, int col, Piece value)
        {
            if (InBounds(row, col))
            {
                grid[row, col] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Set(int row, int col, Piece piece)
        {
            if (InBounds(row, col))
            {
                grid[row, col] = piece;
            }
            else
            {
                throw new IndexOutOfRangeException("Спроба запису за межами дошки.");
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
                (grid[a.Row, a.Col], grid[b.Row, b.Col]) = (grid[b.Row, b.Col], grid[a.Row, a.Col]);
            }
            else
            { 
                throw new IndexOutOfRangeException("Спроба спроба свопу з не існуючим елементом.");
            }
        }

       public IEnumerable<Pos> AllPositions()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    yield return new Pos(row, col);
                }
            }
        }

        public string ToDebugString()
        {
            StringBuilder str = new StringBuilder();
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                   str.Append(PieceTypeMapping(grid[row, col]));
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

    }
}