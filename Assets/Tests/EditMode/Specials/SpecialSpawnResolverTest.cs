using System;
using System.Collections.Generic;
using Core.Matching;
using Core.Specials;
using NUnit.Framework;

namespace Tests.EditMode.Specials
{
    public class SpecialSpawnResolverTest
    {
        [Test]
        public void BuildDecisions_Line4_CreatesSingleDecision()
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

            Assert.AreEqual(1, decisions.Count);
            Assert.AreEqual(decisions[0].SpawnPiece.Kind, PieceKind.RowClear);
            Assert.AreEqual(decisions[0].ConsumedCells.Count, 4);
        }

        [Test]
        public void BuildDecisions_Line5_CreatesColorBombDecision()
        {
            var board = new BoardModel(3, 5);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
            board.Set(0, 4, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
            board.Set(1, 4, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            board.Set(2, 4, Piece.Normal(PieceColor.Red));

            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches);

            Assert.AreEqual(1, decisions.Count);
            Assert.AreEqual(decisions[0].SpawnPiece.Kind, PieceKind.ColorBomb);
            Assert.IsNull(decisions[0].SpawnPiece.Color);
            Assert.AreEqual(decisions[0].ConsumedCells.Count, 5);
        }

        [Test]
        public void BuildDecisions_TShape_CreatesAreaBombDecision()
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

            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches);

            Assert.AreEqual(1, decisions.Count);
            Assert.AreEqual(decisions[0].SpawnPiece.Kind, PieceKind.AreaBomb);
            Assert.AreEqual(decisions[0].ConsumedCells.Count, 5);
        }

        [Test]
        public void BuildDecisions_UsesMatchColorForColoredSpecials()
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

            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches);

            Assert.AreEqual(1, decisions.Count);
            Assert.AreEqual(decisions[0].SpawnPiece.Kind, PieceKind.AreaBomb);
            Assert.AreEqual(decisions[0].SpawnPiece.Color, PieceColor.Green);
        }

        [Test]
        public void BuildDecisions_UsesDeterministicSpawnPosition_WhenMoveNotProvided()
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


            var matches = MatchFinder.FindMatches(board);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches);

            Assert.AreEqual(1, decisions.Count);
            Assert.AreEqual(decisions[0].SpawnPosition, new Pos(0, 2));
            Assert.AreEqual(decisions[0].SpawnPiece.Color, PieceColor.Green);
        }

        [Test]
        public void BuildDecisions_PrefersMoveTarget_WhenMoveProvided()
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

            var matches = MatchFinder.FindMatches(board);
            Pos a = new Pos(1, 3);
            Pos b = new Pos(1, 2);
            var move = new Move(a, b);
            var decisions = SpecialSpawnResolver.BuildDecisions(board, matches, move);

            Assert.AreEqual(1, decisions.Count);
            Assert.AreEqual(decisions[0].SpawnPosition, move.To);
        }

        [Test]
        public void BuildDecisions_Throws_WhenTwoSpawnsTargetSameCell()
        {
            var board = new BoardModel(4, 4);
            
            board.Set(0, 2, Piece.Normal(PieceColor.Green));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
            
            board.Set(2, 2, Piece.Normal(PieceColor.Green));

            board.Set(3, 2, Piece.Normal(PieceColor.Green));

            var matches = new List<Match>
            {
                new Match(new List<Pos>
                {
                    new Pos(1, 0),
                    new Pos(1, 1),
                    new Pos(1, 2),
                    new Pos(1, 3)
                }),
                new Match(new List<Pos>
                {
                    new Pos(0, 2),
                    new Pos(1, 2),
                    new Pos(2, 2),
                    new Pos(3, 2)
                })
            };
            var move = new Move(new Pos(0, 0), new Pos(1, 2));

            Assert.Throws<InvalidOperationException>(() =>
                SpecialSpawnResolver.BuildDecisions(board, matches, move));
        }
    }
}