using System;

namespace Match3.Unity.Save
{
    [Serializable]
    public sealed class LevelRunResultDto
    {
        public string LevelId;
        public string Result;
        public int TurnsUsed;

        public int ManualHintsShown;
        public int IdleHintsShown;
        public int HintsApplied;

        public int TotalClearedCells;
        public int DestroyedObstacles;
        public string CompletedAtUtc;
    }
}