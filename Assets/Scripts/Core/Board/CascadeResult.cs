using System.Collections.Generic;

namespace Core.Board
{
    public  class CascadeResult
    {
        public int  CascadeCount { get; }
        public int TotalClearedCells { get; }
        public bool HadAnyMatches { get; }
        public List<int> ClearedPerWave { get; }

        public CascadeResult( int cascadeCount, int totalClearedCells,  bool hasAnyMatches,  List<int> clearedPerWave)
        {
            CascadeCount = cascadeCount;
            TotalClearedCells = totalClearedCells;
            HadAnyMatches = hasAnyMatches;
            ClearedPerWave = clearedPerWave;
        }
    }
}