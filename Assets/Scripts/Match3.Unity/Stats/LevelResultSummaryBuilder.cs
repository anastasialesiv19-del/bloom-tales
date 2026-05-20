using System.Collections.Generic;
using System.Linq;
using Match3.Unity.Save;

namespace Match3.Unity.Stats
{
    public static class LevelResultSummaryBuilder
    {
        public static IReadOnlyList<LevelResultSummary> Build(
            IReadOnlyList<LevelRunResultDto> results)
        {
            if (results == null || results.Count == 0)
                return new List<LevelResultSummary>();

            return results
                .GroupBy(result => result.LevelId)
                .OrderBy(group => group.Key)
                .Select(BuildForLevel)
                .ToList();
        }

        private static LevelResultSummary BuildForLevel(
            IGrouping<string, LevelRunResultDto> group)
        {
            var items = group.ToList();

            var victories = items.Count(IsVictory);
            var defeats = items.Count(IsDefeat);

            var bestTurns = items
                .Where(IsVictory)
                .Select(item => item.TurnsUsed)
                .DefaultIfEmpty(-1)
                .Min();

            var last = items
                .OrderBy(item => item.CompletedAtUtc)
                .LastOrDefault();

            return new LevelResultSummary(
                group.Key,
                items.Count,
                victories,
                defeats,
                bestTurns,
                items.Sum(item => item.ManualHintsShown),
                items.Sum(item => item.IdleHintsShown),
                items.Sum(item => item.HintsApplied),
                items.Sum(item => item.TotalClearedCells),
                items.Sum(item => item.DestroyedObstacles),
                last?.Result ?? "None");
        }

        private static bool IsVictory(LevelRunResultDto result)
        {
            return result.Result == "Victory";
        }

        private static bool IsDefeat(LevelRunResultDto result)
        {
            return result.Result == "Defeat";
        }
    }
}