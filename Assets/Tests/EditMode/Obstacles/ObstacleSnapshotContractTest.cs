using Core.Board;
using Core.Contracts;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Obstacles
{
    public class ObstacleSnapshotContractTest
    {
        [Test]
        public void Capture_WhenBoardHasObstacle_CapturesObstacle()
        {
            var board = new BoardModel(3, 3);

            board.SetObstacle(1, 1, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });

            var snapshot = BoardSnapshotFactory.Capture(board);

            Assert.IsTrue(snapshot.HasObstacle(1, 1));
            Assert.AreEqual(ObstacleKind.Box, snapshot.GetObstacle(1, 1).Kind);
            Assert.AreEqual(1, snapshot.GetObstacle(1, 1).Health);
        }

        [Test]
        public void Capture_WhenBoardObstacleChangesAfterCapture_SnapshotStaysDetached()
        {
            var board = new BoardModel(3, 3);

            board.SetObstacle(1, 1, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });

            var snapshot = BoardSnapshotFactory.Capture(board);

            board.SetObstacle(1, 1, ObstacleModel.Empty);

            Assert.IsTrue(snapshot.HasObstacle(1, 1));
            Assert.AreEqual(ObstacleKind.Box, snapshot.GetObstacle(1, 1).Kind);
        }

        [Test]
        public void ToDebugString_WhenCellHasObstacle_ShowsObstacleMarker()
        {
            var board = new BoardModel(3, 3);

            board.SetObstacle(1, 1, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });

            var snapshot = BoardSnapshotFactory.Capture(board);

            StringAssert.Contains("BX", snapshot.ToDebugString());
        }
    }
}