using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Specials
{
    public readonly struct SpecialSpawnDecision
    {
        public Pos SpawnPosition { get; }
        public Piece SpawnPiece { get; }
        public IReadOnlyList<Pos> ConsumedCells { get; }

        public SpecialSpawnDecision(Pos spawnPosition, Piece spawnPiece, IReadOnlyList<Pos> consumedCells)
        {
            SpawnPosition = spawnPosition;
            if(spawnPiece.Kind == PieceKind.Empty) throw new ArgumentException("Spawn piece cannot be empty");
            SpawnPiece = spawnPiece;
            if(consumedCells == null || consumedCells.Count == 0) throw new ArgumentException("Consumed cells cannot be null or empty");
            ConsumedCells = consumedCells;
        }
    }
}