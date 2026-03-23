namespace Core.Turns
{
    public readonly struct TurnPhaseTrace
    {
        public TurnPhaseType Type { get;  }
        public int ClearedCells { get; }
        public int SpawnedSpecials { get; }
        public int CascadeIndex { get; }
        public bool BoardShuffled { get; }

        public TurnPhaseTrace(TurnPhaseType type, int clearedCells = 0, int spawnedSpecials = 0, int cascadeIndex = 0, bool boardShuffled = false)
        {
            Type = type;
            ClearedCells = clearedCells;
            SpawnedSpecials = spawnedSpecials;
            CascadeIndex = cascadeIndex;
            BoardShuffled = boardShuffled;
        }
        
        
    }
}