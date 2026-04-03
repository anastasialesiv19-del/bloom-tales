using System;

namespace Core.Turns
{
    public readonly struct TurnPhaseTrace
    {
        public TurnPhaseType Type { get;  }
        public int ClearedCells { get; }
        public int SpawnedSpecials { get; }
        public int CascadeIndex { get; }
        public bool BoardShuffled { get; }

        public TurnPhaseTrace(TurnPhaseType type, int clearedCells = 0, 
            int spawnedSpecials = 0, int cascadeIndex = 0, bool boardShuffled = false)
        {
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
        }
    }
}