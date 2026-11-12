namespace Match3.Unity.Stats
{
    public sealed class LevelResultSummary
    {
        public string LevelId { get; }
        public int Attempts { get; }
        public int Victories { get; }
        public int Defeats { get; }
        public int BestTurnsToVictory { get; }
        public int ManualHintsShown { get; }
        public int IdleHintsShown { get; }
        public int HintsApplied { get; }
        public int TotalClearedCells { get; }
        public int DestroyedObstacles { get; }
        public string LastResult { get; }

        public LevelResultSummary(string levelId, int attempts, int victories, int defeats, int bestTurnsToVictory, int manualHintsShown,
            int idleHintsShown, int hintsApplied, int totalClearedCells, int destroyedObstacles, string lastResult)
        {
            LevelId = levelId;
            Attempts = attempts;
            Victories = victories;
            Defeats = defeats;
            BestTurnsToVictory = bestTurnsToVictory;
            ManualHintsShown = manualHintsShown;
            IdleHintsShown = idleHintsShown;
            HintsApplied = hintsApplied;
            TotalClearedCells = totalClearedCells;
            DestroyedObstacles = destroyedObstacles;
            LastResult = lastResult;
        }
    }
}