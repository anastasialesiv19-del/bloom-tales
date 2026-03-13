using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Board
{
    public class BoardModel
    {
        public int Width { get; }
        public int Height { get; }

        private readonly PieceType[,] grid;

        public BoardModel(int height, int width)
        {
            if (width <= 0 || height <= 0)
            {
                throw new  ArgumentOutOfRangeException("Спроба введення некоректних меж дошки.");

            }
            Width = width;
            Height = height;
            grid = new PieceType[height, width];
        }

        public PieceType Get(int row, int col)
        {
            if (InBounds(row, col))
            {
                return grid[row, col];
            }
            else{
                throw new IndexOutOfRangeException("Спроба доступу до не існуючого елемента.");
            }
            
        }

        public bool TryGet(int row, int col, out PieceType pieceType)
        {
            if (InBounds(row, col))
            {
                pieceType = grid[row, col];
                return true;
            }
            else
            {
                 pieceType = PieceType.Empty;
                 return false;
            }
        }

        public bool TrySet(int row, int col, PieceType value)
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

        public void Set(int row, int col, PieceType pieceType)
        {
            if (InBounds(row, col))
            {
                grid[row, col] = pieceType;
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

        private static char PieceTypeMapping(PieceType type)
        {
            switch (type)
            {
                case PieceType.Empty:
                    return '_';
                case PieceType.Red:
                    return 'R';
                case PieceType.Blue:
                    return 'B';
                case PieceType.Yellow:
                    return 'Y';
                case PieceType.Green:
                    return 'G';
                default:
                    throw new InvalidOperationException();
            }
            
        }

    }
}