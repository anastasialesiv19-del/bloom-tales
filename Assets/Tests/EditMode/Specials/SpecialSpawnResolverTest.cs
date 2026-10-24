using System;
using System.Collections.Generic;
using Core.Board;
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

        [Test]
        public void BuildDecisions_WhenMultiplePiecesFallIntoMatch_UsesLongestFallDestination()
        {
            var board = new BoardModel(4, 4);
            
            board.Set(3, 0, Piece.Normal(PieceColor.Red));
            board.Set(3, 1, Piece.Normal(PieceColor.Red));
            board.Set(3, 2, Piece.Normal(PieceColor.Red));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var match = new List<Match>
            {
                new Match(new List<Pos>
                {
                    new Pos(3, 0),
                    new Pos(3, 1),
                    new Pos(3, 2),
                    new Pos(3, 3)
                })
            };
            
            var fallsBeforeWave = new List<PieceFall>
            {
                new PieceFall(
                    new Pos(2,0),
                    new Pos(3, 0)
                ),
                new PieceFall(
                    new Pos(0,2),
                    new Pos(3, 2)
                ),
                new PieceFall(
                    new Pos(1,3),
                    new Pos(3, 3)
                )
            };

            var result = SpecialSpawnResolver.BuildDecisions(board, match, fallsBeforeWave: fallsBeforeWave);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new Pos(3,2), result[0].SpawnPosition);
        }
        
        [Test]
        public void BuildDecisions_WhenFallsHaveEqualDistanceInSameRow_UsesLowestColumn()
        {
            var board = new BoardModel(4, 4);
            
            board.Set(3, 0, Piece.Normal(PieceColor.Red));
            board.Set(3, 1, Piece.Normal(PieceColor.Red));
            board.Set(3, 2, Piece.Normal(PieceColor.Red));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var match = new List<Match>
            {
                new Match(new List<Pos>
                {
                    new Pos(3, 0),
                    new Pos(3, 1),
                    new Pos(3, 2),
                    new Pos(3, 3)
                })
            };
            
            var fallsBeforeWave1 = new List<PieceFall>
            {
                new PieceFall(
                    new Pos(1,3),
                    new Pos(3, 3)
                ),
                new PieceFall(
                    new Pos(1,1),
                    new Pos(3, 1)
                )
            };

            var result1 = SpecialSpawnResolver.BuildDecisions(board, match, fallsBeforeWave: fallsBeforeWave1);

            Assert.AreEqual(1, result1.Count);
            Assert.AreEqual(new Pos(3,1), result1[0].SpawnPosition);
            
            var fallsBeforeWave2 = new List<PieceFall>
            {
                new PieceFall(
                    new Pos(1, 1),
                    new Pos(3, 1)
                ),
                new PieceFall(
                    new Pos(1, 3),
                    new Pos(3, 3)
                )
            };

            var result2 = SpecialSpawnResolver.BuildDecisions(board, match, fallsBeforeWave: fallsBeforeWave2);

            Assert.AreEqual(1, result2.Count);
            Assert.AreEqual(new Pos(3,1), result2[0].SpawnPosition);
        }
        
        [Test]
        public void BuildDecisions_WhenFallsHaveEqualDistanceInSameColumn_UsesLowestRow()
        {
            var board = new BoardModel(5, 4);
            
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(2, 2, Piece.Normal(PieceColor.Red));
            board.Set(3, 2, Piece.Normal(PieceColor.Red));
            board.Set(4, 2, Piece.Normal(PieceColor.Red));

            var match = new List<Match>
            {
                new Match(new List<Pos>
                {
                    new Pos(1, 2),
                    new Pos(2, 2),
                    new Pos(3, 2),
                    new Pos(4, 2)
                })
            };
            
            var fallsBeforeWave1 = new List<PieceFall>
            {
                new PieceFall(
                    new Pos(2, 2),
                    new Pos(4, 2)
                ),
                new PieceFall(
                    new Pos(0, 2),
                    new Pos(2, 2)
                )
            };

            var result1 = SpecialSpawnResolver.BuildDecisions(board, match, fallsBeforeWave: fallsBeforeWave1);

            Assert.AreEqual(1, result1.Count);
            Assert.AreEqual(new Pos(2,2), result1[0].SpawnPosition);
            
            var fallsBeforeWave2 = new List<PieceFall>
            {
                new PieceFall(
                    new Pos(0, 2),
                    new Pos(2, 2)
                ),
                new PieceFall(
                    new Pos(2, 2),
                    new Pos(4, 2)
                )
            };

            var result2 = SpecialSpawnResolver.BuildDecisions(board, match, fallsBeforeWave: fallsBeforeWave2);

            Assert.AreEqual(1, result2.Count);
            Assert.AreEqual(new Pos(2,2), result2[0].SpawnPosition);
        }

        [Test]
        public void BuildDecisions_WhenMatchContainsRefilledPiece_PrefersItOverFallenPiece()
        {
            var board = new BoardModel(4, 4);
            
            board.Set(0, 0, Piece.Normal(PieceColor.Green));
            
            board.Set(3, 0, Piece.Normal(PieceColor.Red));
            board.Set(3, 1, Piece.Normal(PieceColor.Red));
            board.Set(3, 2, Piece.Normal(PieceColor.Red));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var match = new List<Match>
            {
                new Match(new List<Pos>
                {
                    new Pos(3, 0),
                    new Pos(3, 1),
                    new Pos(3, 2),
                    new Pos(3, 3)
                })
            };
            
            var fallsBeforeWave = new List<PieceFall>
            {
                new PieceFall(
                    new Pos(0, 2),
                    new Pos(3, 2)
                )
            };
            
            var refillsBeforeWave = new List<Pos>
            {
               new Pos(0, 0),
               new Pos(3, 3)
            };

            var result = SpecialSpawnResolver.BuildDecisions(board, match, fallsBeforeWave: fallsBeforeWave, refillsBeforeWave: refillsBeforeWave);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new Pos(3,3), result[0].SpawnPosition);
        }
        
        [Test]
        public void BuildDecisions_WhenRefillsAreOutsideMatch_UsesFallenPieceDestination()
        {
            var board = new BoardModel(4, 4);
            
            board.Set(0, 0, Piece.Normal(PieceColor.Green));
            
            board.Set(3, 0, Piece.Normal(PieceColor.Red));
            board.Set(3, 1, Piece.Normal(PieceColor.Red));
            board.Set(3, 2, Piece.Normal(PieceColor.Red));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var match = new List<Match>
            {
                new Match(new List<Pos>
                {
                    new Pos(3, 0),
                    new Pos(3, 1),
                    new Pos(3, 2),
                    new Pos(3, 3)
                })
            };
            
            var fallsBeforeWave = new List<PieceFall>
            {
                new PieceFall(
                    new Pos(0, 2),
                    new Pos(3, 2)
                )
            };
            
            var refillsBeforeWave = new List<Pos>
            {
                new Pos(0, 0)
            };

            var result = SpecialSpawnResolver.BuildDecisions(board, match, fallsBeforeWave: fallsBeforeWave, refillsBeforeWave: refillsBeforeWave);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new Pos(3,2), result[0].SpawnPosition);
        }
    }
}