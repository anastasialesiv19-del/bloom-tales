using System.Collections.Generic;
using Core.Board;
using Match3.Unity.Goals;
using NUnit.Framework;

namespace Tests.EditMode.Goals
{
    public class GoalProgressStatsTest
    {
        [Test]
        public void GoalProgress_WhenAppliedStats_AddsTotalClearedCells()
        {
            var goal = new GoalProgress(5);
            var stats = new ClearResolutionStats(3, new Dictionary<PieceColor, int>(), 0);
            
            goal.Apply(stats);

            Assert.AreEqual(3, goal.Current);
        }
        
        [Test]
        public void ColorGoalProgress_WhenAppliedStats_AddsOnlyMatchingColor()
        {
            var goal = new ColorGoalProgress(PieceColor.Red, 5);
            var stats = new ClearResolutionStats(3, new Dictionary<PieceColor, int>
                {
                    { PieceColor.Red, 2 },
                    { PieceColor.Blue, 1 }
                }, 0);

            goal.Apply(stats);

            Assert.AreEqual(2, goal.Current);
        }
        
        [Test]
        public void ObstacleGoalProgress_WhenAppliedStats_AddsDestroyedObstacles()
        {
            var goal = new ObstacleGoalProgress(ObstacleKind.Box, 2);
            var stats = new ClearResolutionStats(1, new Dictionary<PieceColor, int>(), 1);

            goal.Apply(stats);

            Assert.AreEqual(1, goal.Current);
        }
    }
}