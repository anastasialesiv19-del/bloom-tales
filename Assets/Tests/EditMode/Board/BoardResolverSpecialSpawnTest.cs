using System.Linq;
using Core.Board;
using Core.Matching;
using Core.Specials;
using NUnit.Framework;

namespace Tests.EditMode.Board
{
    public class BoardResolverSpecialSpawnTest
    {
        [Test]
        public void ClearMatchesAndSpawnSpecials_ClearsAllConsumedCells()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Red));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            Pos a = new Pos(1, 3);
            Pos b = new Pos(1, 2);
            var move = new Move(a, b);

            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches, move);
            BoardResolver.ClearMatchesAndSpawnSpecials(board, matches, decisions);
            var spawnPositions = decisions.Select(x => x.SpawnPosition).ToList();
            foreach (var decision in decisions)
            foreach (var pos in decision.ConsumedCells)
            {
                bool isAnySpawnPosition = spawnPositions.Any(x => x.Equals(pos));
                if (!isAnySpawnPosition)
                    Assert.IsTrue(board.Get(pos.Row, pos.Col).IsEmpty,
                        $"Cell ({pos.Row},{pos.Col}) is not empty.\n{board.ToDebugString()}");
            }
        }

        [Test]
        public void ClearMatchesAndSpawnSpecials_LeavesSpecialPieceAtSpawnPosition()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Red));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            Pos a = new Pos(1, 3);
            Pos b = new Pos(1, 2);
            var move = new Move(a, b);

            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board, matches, decisions);
            var piece = board.Get(decisions[0].SpawnPosition.Row, decisions[0].SpawnPosition.Col);
            
            Assert.AreEqual(piece.Kind, decisions[0].SpawnPiece.Kind);
            Assert.AreEqual(piece.Color, decisions[0].SpawnPiece.Color);
            Assert.AreEqual(piece.IsSpecial, decisions[0].SpawnPiece.IsSpecial);
        }

        [Test]
        public void ClearMatchesAndSpawnSpecials_SpawnPositionIsNotEmptyAfterResolution()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Red));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            Pos a = new Pos(1, 3);
            Pos b = new Pos(1, 2);
            var move = new Move(a, b);

            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches);

            Assert.IsFalse(board.Get(decisions[0].SpawnPosition.Row, decisions[0].SpawnPosition.Col).IsEmpty);
        }
    }
}