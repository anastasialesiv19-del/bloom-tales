using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class TurnProcessorTest
    {
        [Test]
        public void TryProcessMove_ReturnsNotApplied_WhenSwapIsOutOfBounds()
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
            
            var move = new Move(new Pos(0,3), new Pos(0,4));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            Assert.IsFalse(turnResult.WasSwapValid);
            Assert.IsFalse(turnResult.WasApplied);
            Assert.IsFalse(turnResult.CreatedMatch);
            Assert.AreEqual(0, turnResult.CascadeCount);
            Assert.AreEqual(0, turnResult.TotalClearedCells);
            Assert.AreEqual(FailureReason.InvalidSwap, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_ReturnsNotApplied_WhenSwapDoesNotCreateMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var move = new Move(new Pos(1,1), new Pos(1,2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            Assert.IsTrue(turnResult.WasSwapValid, board.ToDebugString());
            Assert.IsFalse(turnResult.WasApplied, board.ToDebugString());
            Assert.IsFalse(turnResult.CreatedMatch, board.ToDebugString());
            Assert.AreEqual(0, turnResult.CascadeCount, board.ToDebugString());
            Assert.AreEqual(0, turnResult.TotalClearedCells, board.ToDebugString());
            Assert.AreEqual(FailureReason.NoMatchCreated, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_DoesNotChangeBoard_WhenSwapDoesNotCreateMatch()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var before = board.ToDebugString();
            var move = new Move(new Pos(1,1), new Pos(1,2));
            TurnProcessor.TryProcessMove(board, move);
            var after = board.ToDebugString();
            
            Assert.AreEqual(before, after);
        }

        [Test]
        public void TryProcessMove_AppliesSwapAndResolvesCascade_WhenSwapCreatesMatch()
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
            
            var move = new Move(new Pos(1,2), new Pos(1,3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            bool itHasEmpty =  BoardResolver.HasEmptyCells(board);
            
            Assert.IsFalse(itHasEmpty, board.ToDebugString());
            Assert.IsTrue(turnResult.WasSwapValid, board.ToDebugString());
            Assert.IsTrue(turnResult.WasApplied, board.ToDebugString());
            Assert.IsTrue(turnResult.CreatedMatch, board.ToDebugString());
            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
        }

        [Test]
        public void TryProcessMove_ReturnsCascadeInfo_WhenMoveSucceeds()
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
            
            var move = new Move(new Pos(1,2), new Pos(1,3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            Assert.IsTrue(turnResult.CascadeCount > 0, board.ToDebugString());
            Assert.IsTrue(turnResult.TotalClearedCells > 0, board.ToDebugString());
            Assert.AreEqual(move,  turnResult.Move);
            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
        }

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
            
            var move = new Move(new Pos(1,2), new Pos(2,2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            bool foundSpecialPiece = false;
            foreach (var pos in board.AllPositions())
            {
                var pieceKind = board.Get(pos.Row, pos.Col).Kind;
                if (pieceKind == PieceKind.RowClear || pieceKind == PieceKind.ColumnClear)
                {
                    foundSpecialPiece = true;
                    Assert.AreEqual(PieceColor.Green, board.Get(pos.Row, pos.Col).Color);
                    break;
                }
            }
            Assert.IsTrue(turnResult.WasSwapValid);
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(foundSpecialPiece);
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
            
            var move = new Move(new Pos(1,2), new Pos(2,2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            bool foundSpecialPiece = false;
            foreach (var pos in board.AllPositions())
            {
                var pieceKind = board.Get(pos.Row, pos.Col).Kind;
                if (pieceKind == PieceKind.ColorBomb)
                {
                    foundSpecialPiece = true;
                    break;
                }
            }
            Assert.IsTrue(turnResult.WasSwapValid);
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(foundSpecialPiece);
        }
        
        [Test]
        public void  ProcessTurn_TShapePrimaryMatch_SpawnsAreaBomb()
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
            
            var move = new Move(new Pos(1,2), new Pos(1,3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            bool foundSpecialPiece = false;
            foreach (var pos in board.AllPositions())
            {
                var pieceKind = board.Get(pos.Row, pos.Col).Kind;
                if (pieceKind == PieceKind.AreaBomb)
                {
                    foundSpecialPiece = true;
                    Assert.AreEqual(PieceColor.Green, board.Get(pos.Row, pos.Col).Color);
                    break;
                }
            }
            Assert.IsTrue(turnResult.WasSwapValid);
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.IsTrue(foundSpecialPiece);
        }

        [Test]
        public void ProcessTurn_CascadeMatches_DoNotSpawnAdditionalSpecials()
        {
            var board = new BoardModel(5, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Red));
            board.Set(2, 3, Piece.Normal(PieceColor.Green));
            
            board.Set(3, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(3, 1, Piece.Normal(PieceColor.Blue));
            board.Set(3, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));
            
            board.Set(4, 0, Piece.Normal(PieceColor.Red));
            board.Set(4, 1, Piece.Normal(PieceColor.Blue));
            board.Set(4, 2, Piece.Normal(PieceColor.Green));
            board.Set(4, 3, Piece.Normal(PieceColor.Yellow));
            
            var move = new Move(new Pos(1,2), new Pos(2,2));
           
            var turnResult = TurnProcessor.TryProcessMove(board, move);

            int countSpecialPiece = 0;
            foreach (var pos in board.AllPositions())
            {
                var pieceKind = board.Get(pos.Row, pos.Col).Kind;
                if (pieceKind == PieceKind.RowClear)
                {
                    countSpecialPiece++;
                    Assert.AreEqual(PieceColor.Green, board.Get(pos.Row, pos.Col).Color);
                    break;
                }
            }
            
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.AreEqual(1, countSpecialPiece);
            Assert.AreEqual(board.Get(2, 2).Kind, PieceKind.RowClear);
            Assert.AreEqual(board.Get(2, 2).Color, PieceColor.Green);
        }

        [Test]
        public void ProcessTurn_SpecialSpawnPosition_IsPreservedThroughResolution()
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
            
            var move = new Move(new Pos(2,2), new Pos(1,2));
            TurnProcessor.TryProcessMove(board, move);
           
            Assert.AreEqual(board.Get(1, 2).Kind, PieceKind.RowClear);
            Assert.IsFalse(board.Get(1, 2).IsEmpty);
        }
    }
}