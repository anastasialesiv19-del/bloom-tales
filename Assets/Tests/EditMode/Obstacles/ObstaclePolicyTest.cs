using Core.Board;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Obstacles
{
    public class ObstaclePolicyTest
    {
        [Test]
        public void MoveValidator_WhenSwapTouchesObstacle_ReturnsFalse()
        {
            var board = new BoardModel(3, 3);
            Fill(board, PieceColor.Red);

            board.SetObstacle(1, 1, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });
            board.Set(1, 1, Piece.Empty());

            var result = MoveValidator.IsValidSwap(
                board,
                new Pos(1, 1),
                new Pos(1, 2));

            Assert.IsFalse(result);
        }

        [Test]
        public void MatchFinder_WhenObstacleBetweenSameColors_DoesNotMatchThroughObstacle()
        {
            var board = new BoardModel(1, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Red));
            board.Set(0, 2, Piece.Empty());
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.SetObstacle(0, 2, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });

            var matches = MatchFinder.FindMatches(board);

            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void BoardResolver_WhenClearAdjacentToObstacle_DamagesObstacle()
        {
            var board = new BoardModel(3, 3);
            Fill(board, PieceColor.Blue);

            board.SetObstacle(1, 1, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });
            board.Set(1, 1, Piece.Empty());

            BoardResolver.ClearCells(board, new[]
            {
                new Pos(1, 0)
            });

            Assert.IsTrue(board.GetObstacle(1, 1).IsEmpty);
        }

        [Test]
        public void BoardResolver_WhenObstacleAdjacentToMultipleClearedCells_TakesOnlyOneDamage()
        {
            var board = new BoardModel(3, 3);
            Fill(board, PieceColor.Blue);

            board.SetObstacle(1, 1, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 2
            });
            board.Set(1, 1, Piece.Empty());

            BoardResolver.ClearCells(board, new[]
            {
                new Pos(1, 0),
                new Pos(0, 1)
            });

            Assert.AreEqual(1, board.GetObstacle(1, 1).Health);
        }

        [Test]
        public void BoardResolver_ApplyGravity_DoesNotMovePieceIntoObstacleCell()
        {
            var board = new BoardModel(4, 1);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 0, Piece.Empty());
            board.Set(2, 0, Piece.Empty());
            board.Set(3, 0, Piece.Empty());

            board.SetObstacle(2, 0, new ObstacleModel
            {
                Kind = ObstacleKind.Box,
                Health = 1
            });

            BoardResolver.ApplyGravity(board);

            Assert.IsFalse(board.Get(1, 0).IsEmpty);
            Assert.IsTrue(board.Get(2, 0).IsEmpty);
            Assert.IsTrue(board.HasObstacle(2, 0));
        }

        private static void Fill(BoardModel board, PieceColor color)
        {
            foreach (var pos in board.AllPositions())
            {
                board.Set(pos.Row, pos.Col, Piece.Normal(color));
            }
        }
    }
}