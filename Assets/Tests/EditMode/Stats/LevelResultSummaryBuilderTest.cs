using System.Linq;
using System.Collections.Generic;
using Match3.Unity.Save;
using Match3.Unity.Stats;
using NUnit.Framework;

namespace Tests.EditMode.Stats
{
    public class LevelResultSummaryBuilderTest
    {
        [Test]
        public void Build_WhenResultsContainMultipleRuns_GroupsByLevel()
        {
            var results = new List<LevelRunResultDto>
            {
                new LevelRunResultDto
                {
                    LevelId = "Level1",
                    Result = "Victory",
                    TurnsUsed = 5,
                    ManualHintsShown = 1,
                    IdleHintsShown = 2,
                    HintsApplied = 1,
                    TotalClearedCells = 20,
                    DestroyedObstacles = 2,
                    CompletedAtUtc = "2026-01-01T00:00:00Z"
                },
                new LevelRunResultDto
                {
                    LevelId = "Level1",
                    Result = "Defeat",
                    TurnsUsed = 8,
                    ManualHintsShown = 0,
                    IdleHintsShown = 1,
                    HintsApplied = 0,
                    TotalClearedCells = 10,
                    DestroyedObstacles = 0,
                    CompletedAtUtc = "2026-01-02T00:00:00Z"
                }
            };

            results.Add(new LevelRunResultDto
            {
                LevelId = "Level2", Result = "Victory", TurnsUsed = 3,
                TotalClearedCells = 7, CompletedAtUtc = "2026-01-03T00:00:00Z"
            });
            var summaries = LevelResultSummaryBuilder.Build(results);

            Assert.AreEqual(2, summaries.Count);
            var second = summaries.Single(summary => summary.LevelId == "Level2");
            Assert.AreEqual(1, second.Attempts);
            Assert.AreEqual(1, second.Victories);
            Assert.AreEqual(0, second.Defeats);
            Assert.AreEqual(7, second.TotalClearedCells);
            Assert.AreEqual(30, summaries.Single(summary => summary.LevelId == "Level1").TotalClearedCells);
            Assert.AreEqual("Level1", summaries[0].LevelId);
            Assert.AreEqual(2, summaries[0].Attempts);
            Assert.AreEqual(1, summaries[0].Victories);
            Assert.AreEqual(1, summaries[0].Defeats);
        }

        [Test]
        public void Build_WhenVictoriesExist_UsesBestVictoryTurns()
        {
            var results = new List<LevelRunResultDto>
            {
                new LevelRunResultDto
                {
                    LevelId = "Level1",
                    Result = "Victory",
                    TurnsUsed = 10,
                    CompletedAtUtc = "2026-01-01T00:00:00Z"
                },
                new LevelRunResultDto
                {
                    LevelId = "Level1",
                    Result = "Victory",
                    TurnsUsed = 4,
                    CompletedAtUtc = "2026-01-02T00:00:00Z"
                }
            };

            var summaries = LevelResultSummaryBuilder.Build(results);

            Assert.AreEqual(4, summaries[0].BestTurnsToVictory);
        }

        [Test]
        public void Build_WhenHintStatsExist_SumsHintCounters()
        {
            var results = new List<LevelRunResultDto>
            {
                new LevelRunResultDto
                {
                    LevelId = "Level1",
                    Result = "Victory",
                    ManualHintsShown = 2,
                    IdleHintsShown = 3,
                    HintsApplied = 1,
                    TotalClearedCells = 10,
                    DestroyedObstacles = 1
                },
                new LevelRunResultDto
                {
                    LevelId = "Level1",
                    Result = "Defeat",
                    ManualHintsShown = 1,
                    IdleHintsShown = 4,
                    HintsApplied = 2,
                    TotalClearedCells = 20,
                    DestroyedObstacles = 2
                }
            };

            var summaries = LevelResultSummaryBuilder.Build(results);

            Assert.AreEqual(3, summaries[0].ManualHintsShown);
            Assert.AreEqual(7, summaries[0].IdleHintsShown);
            Assert.AreEqual(3, summaries[0].HintsApplied);
            Assert.AreEqual(30, summaries[0].TotalClearedCells);
            Assert.AreEqual(3, summaries[0].DestroyedObstacles);
        }
    }
}
