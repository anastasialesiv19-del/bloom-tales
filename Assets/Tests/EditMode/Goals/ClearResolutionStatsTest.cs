using System.Collections.Generic;
using Core.Board;
using Core.Contracts;
using Core.Presentation;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Goals
{
    public class ClearResolutionStatsTest
    {
        [Test]
        public void ClearCellsWithStats_WhenClearingColoredPieces_CountsColors()
        {
            var board = new BoardModel(1, 3);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Red));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));

            var stats = BoardResolver.ClearCells(board, new[] { new Pos(0, 0), new Pos(0, 1), new Pos(0, 2) });

            Assert.AreEqual(3, stats.TotalClearedCells);
            Assert.AreEqual(2, stats.GetClearedCount(PieceColor.Red));
            Assert.AreEqual(1, stats.GetClearedCount(PieceColor.Blue));
        }
        
        [Test]
        public void ClearCellsWithStats_WhenAdjacentObstacleDestroyed_CountsDestroyedObstacle()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.SetObstacle(0, 1, new ObstacleModel { Kind = ObstacleKind.Box, Health = 1 });

            var stats = BoardResolver.ClearCells(board, new[] { new Pos(0, 0) });

            Assert.AreEqual(1, stats.TotalClearedCells);
            Assert.AreEqual(1, stats.DestroyedObstacles);
        }
        
        [Test]
        public void ClearCellsWithStats_WhenObstacleAdjacentToMultipleClearedCells_CountsDestroyedOnce()
        {
            var board = new BoardModel(2, 2);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.SetObstacle(0, 1, new ObstacleModel { Kind = ObstacleKind.Box, Health = 1 });

            var stats = BoardResolver.ClearCells(board, new[] { new Pos(0, 0), new Pos(1, 1) });

            Assert.AreEqual(2, stats.TotalClearedCells);
            Assert.AreEqual(1, stats.DestroyedObstacles);
        }
        
        [Test]
        public void TurnPresentationFactory_WhenResultHasClearStats_ForwardsStats()
        {
            var board = new BoardModel(1, 1);
            var snapshot = BoardSnapshotFactory.Capture(board);

            var stats = new ClearResolutionStats(2, new Dictionary<PieceColor, int> { { PieceColor.Red, 2 } }, 1);

            var result = new TurnResult(true, true, false, true, true,
                0, 2, new Move(new Pos(0, 0), new Pos(0, 0)), FailureReason.None,
                new TurnTrace(new[]
                {
                    new TurnPhaseTrace(TurnPhaseType.TurnCompleted)
                }),
                snapshot, snapshot, stats);

            var presentation = TurnPresentationFactory.Create(result);

            Assert.AreEqual(2, presentation.ClearStats.GetClearedCount(PieceColor.Red));
            Assert.AreEqual(1, presentation.ClearStats.DestroyedObstacles);
        }
    }
}