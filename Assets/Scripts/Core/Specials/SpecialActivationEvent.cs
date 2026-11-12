using System;
using System.Collections.Generic;
using Core.Board;

namespace Core.Specials
{
    public readonly struct SpecialActivationEvent
    {
        public Pos Position { get; }
        public Piece Piece { get; }
        public int ChainDepth { get; }
        public IReadOnlyList<Pos> AffectedCells { get; }

        public SpecialActivationEvent(Pos position, Piece piece, int chainDepth, IReadOnlyList<Pos> affectedCells)
        {
            if (chainDepth < 0)
            {
                throw new ArgumentException(nameof(chainDepth));
            }
            if (!piece.IsSpecial)
            {
                throw new ArgumentException(nameof(piece));
            }
            if (affectedCells == null)
            {
                throw new ArgumentNullException(nameof(affectedCells));
            }
            
            Position = position;
            Piece = piece;
            ChainDepth = chainDepth;
            AffectedCells = new List<Pos>(affectedCells).AsReadOnly();
        }
    }
}