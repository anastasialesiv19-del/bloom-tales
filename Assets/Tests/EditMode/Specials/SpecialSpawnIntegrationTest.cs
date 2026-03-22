using Core.Board;
using Core.Matching;
using Core.Specials;
using NUnit.Framework;

namespace Tests.EditMode.Specials
{
    public class SpecialSpawnIntegrationTest
    {
        [Test]
        public void ResolveSpecialMatch_Line4_SpawnsSpecialPieceOnBoard()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Red));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches);
            BoardResolver.ClearMatchesAndSpawnSpecials(board, matches, decisions);
            var piece = board.Get(decisions[0].SpawnPosition.Row, decisions[0].SpawnPosition.Col);
            
            Assert.AreEqual(piece.Kind, PieceKind.RowClear);
            
        }
    }
}