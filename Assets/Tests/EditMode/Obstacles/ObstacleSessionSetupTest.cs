using System.Collections.Generic;
using Core.Board;
using NUnit.Framework;
using Match3.Unity.GameFlow;

namespace Tests.EditMode.Obstacles
{
    public class ObstacleSessionSetupTest
    {
        [Test]
        public void StartNewGame_WhenObstacleDefinitionsProvided_PlacesObstacles()
        {
            var session = new GameSession();
            var snapshot = session.StartNewGame(6, 6, new List<ObstacleDefinition> { new(new Pos(2, 2), ObstacleKind.Box, 1) });

            Assert.IsTrue(snapshot.HasObstacle(2, 2));
            Assert.AreEqual(ObstacleKind.Box, snapshot.GetObstacle(2, 2).Kind);
            Assert.IsTrue(snapshot.Get(2, 2).IsEmpty);
        }

        [Test]
        public void StartNewGame_WhenObstacleDefinitionOutOfBounds_IgnoresIt()
        {
            var session = new GameSession();
            var snapshot = session.StartNewGame(6, 6, new List<ObstacleDefinition> { new(new Pos(99, 99), ObstacleKind.Box, 1) });

            Assert.DoesNotThrow(() => snapshot.Get(0, 0));
        }
    }

}