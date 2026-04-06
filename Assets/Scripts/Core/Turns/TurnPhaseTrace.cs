using System;
using Core.Contracts;

namespace Core.Turns
{
    public readonly struct TurnPhaseTrace
    {
        public TurnPhaseType Type { get;  }
        public int ClearedCells { get; }
        public int SpawnedSpecials { get; }
        public int CascadeIndex { get; }
        public bool BoardShuffled { get; }
        
        public BoardSnapshot BoardAfterPhase { get; }

        public TurnPhaseTrace(TurnPhaseType type, int clearedCells = 0, 
            int spawnedSpecials = 0, int cascadeIndex = 0, bool boardShuffled = false, BoardSnapshot boardAfterPhase = null) : this()
        {
            if (RequiresBoardSnapshot(type) && boardAfterPhase == null)
                throw new ArgumentException("Mutating phase must has a snapshot.");
            if(clearedCells < 0)
                throw new ArgumentException("Cleared cells cannot have negative values.");
            if(spawnedSpecials < 0)
                throw new ArgumentException("Spawned specials cannot have negative values.");
            if(cascadeIndex < 0)
                throw new ArgumentException("Cascade index cannot have negative values.");
            
            if(type != TurnPhaseType.CascadeWaveResolved && cascadeIndex > 0)
                throw new ArgumentException("Type that is not CascadeWaveResolved cannot have cascade index.");
            if(type == TurnPhaseType.CascadeWaveResolved && cascadeIndex == 0)
                throw new ArgumentException("Type that is CascadeWaveResolved must have cascade index.");

            Type = type;
            ClearedCells = clearedCells;
            SpawnedSpecials = spawnedSpecials;
            CascadeIndex = cascadeIndex;
            BoardShuffled = boardShuffled;
            BoardAfterPhase = boardAfterPhase;
        }

        private static bool RequiresBoardSnapshot(TurnPhaseType type)
        {
            switch (type)
            {
                case TurnPhaseType.SwapApplied:
                case TurnPhaseType.DirectSpecialActivation:
                case TurnPhaseType.PrimaryMatchResolved:
                case TurnPhaseType.GravityApplied:
                case TurnPhaseType.RefillApplied:
                case TurnPhaseType.CascadeWaveResolved:
                case TurnPhaseType.ShuffleApplied: return true;
                case TurnPhaseType.InvalidSwap:
                case TurnPhaseType.NoMatchCreated:
                case TurnPhaseType.TurnCompleted:
                default: return false;
            }
        }
    }
}