using System.Collections.Generic;
using Tests.EditMode.TestDoubles;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Turns
{
    public class TurnProcessorSpecialSpawnTest
    {
        [Test]
        public void ProcessTurn_Line4PrimaryMatch_SpawnsSpecialPiece()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(2, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(TurnTraceQueries.TryGetPhase(turnResult.Trace,
                TurnPhaseType.PrimaryMatchResolved, out var phase));
            Assert.AreEqual(1, phase.SpawnedSpecials);
            var piece = phase.BoardAfterPhase.Get(1, 2);
            Assert.AreEqual(PieceKind.RowClear, piece.Kind);
            Assert.AreEqual(PieceColor.Green, piece.Color);
        }

        [Test]
        public void ProcessTurn_Line5PrimaryMatch_SpawnsSpecialPiece()
        {
            var board = new BoardModel(3, 5);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
            board.Set(0, 4, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
            board.Set(1, 4, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            board.Set(2, 4, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(2, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(TurnTraceQueries.TryGetPhase(turnResult.Trace,
                TurnPhaseType.PrimaryMatchResolved, out var phase));
            Assert.AreEqual(1, phase.SpawnedSpecials);
            var piece = phase.BoardAfterPhase.Get(1, 2);
            Assert.AreEqual(PieceKind.ColorBomb, piece.Kind);
            Assert.IsNull(piece.Color);
        }

        [Test]
        public void ProcessTurn_TShapePrimaryMatch_SpawnsAreaBomb()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());

            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(TurnTraceQueries.TryGetPhase(turnResult.Trace,
                TurnPhaseType.PrimaryMatchResolved, out var phase));
            Assert.AreEqual(1, phase.SpawnedSpecials);
            var piece = phase.BoardAfterPhase.Get(1, 2);
            Assert.AreEqual(PieceKind.AreaBomb, piece.Kind);
            Assert.AreEqual(PieceColor.Green, piece.Color);
        }

        [Test]
        public void ProcessTurn_PrimarySpecialSpawn_UsesMoveDestination()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(2, 2), new Pos(1, 2));
            var result = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
            Assert.IsTrue(result.WasApplied);

            Assert.IsTrue(TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.PrimaryMatchResolved, out var phase));

            Assert.AreEqual(1, phase.SpawnedSpecials);

            var spawnedPiece = phase.BoardAfterPhase.Get(1, 2);

            Assert.AreEqual(PieceKind.RowClear, spawnedPiece.Kind);
            Assert.AreEqual(PieceColor.Green, spawnedPiece.Color);
        }

        [Test]
        public void ProcessTurn_WhenGravityFormsLine4_CreatesRowClearInCascade()
        {
            var board = new BoardModel(4, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Green));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));
            
            board.Set(3, 0, Piece.Normal(PieceColor.Red));
            board.Set(3, 1, Piece.Normal(PieceColor.Red));
            board.Set(3, 2, Piece.Normal(PieceColor.Blue));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(2, 2), new Pos(2, 3));
            var result = TurnProcessor.TryProcessMove(board, move, new SequenceRandomProvider(new[] { 4, 2, 1, 0, 4, 2 }));
            
            Assert.IsTrue(result.WasApplied);
            Assert.AreEqual(1, result.CascadeCount);
            
            Assert.IsTrue(TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.PrimaryMatchResolved, out var phase));
            Assert.AreEqual(3, phase.ClearedCells);
            Assert.AreEqual(0, phase.SpawnedSpecials);
            
            Assert.IsTrue(TurnTraceQueries.TryGetPhase(result.Trace, TurnPhaseType.CascadeWaveResolved, out var cascadePhase));
            Assert.AreEqual(1, cascadePhase.CascadeIndex);
            Assert.AreEqual(4, cascadePhase.ClearedCells);
            
            var boardAfterPhase = cascadePhase.BoardAfterPhase;
            Assert.IsNotNull(boardAfterPhase);
            var specialPieces = new List<Piece>();
            for (int row = 0; row < boardAfterPhase.Height; row++)
            {
                for (int col = 0; col < boardAfterPhase.Width; col++)
                {
                    if(boardAfterPhase.Get(row, col).IsSpecial)
                        specialPieces.Add(boardAfterPhase.Get(row, col));
                }
            }
            Assert.AreEqual(1, specialPieces.Count);
            Assert.AreEqual(PieceKind.RowClear, specialPieces[0].Kind);
            Assert.AreEqual(PieceColor.Red, specialPieces[0].Color);
            Assert.AreEqual(PieceKind.RowClear, boardAfterPhase.Get(3, 2).Kind);
            Assert.AreEqual(PieceColor.Red, boardAfterPhase.Get(3, 2).Color);
        }
    }
}
