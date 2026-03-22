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

            var move = new Move(new Pos(1, 2), new Pos(2, 2));
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

            var move = new Move(new Pos(1, 2), new Pos(2, 2));

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

            var move = new Move(new Pos(2, 2), new Pos(1, 2));
            TurnProcessor.TryProcessMove(board, move);

            Assert.AreEqual(board.Get(1, 2).Kind, PieceKind.RowClear);
            Assert.IsFalse(board.Get(1, 2).IsEmpty);
        }
    }
}