using System.Linq;
using Core.Matching;
using Core.Presentation;
using Core.Turns;
using NUnit.Framework;
using Tests.EditMode.TestDoubles;

namespace Tests.EditMode.Turns
{
    public class TurnPresentationFactoryTest
    {
        [Test]
        public void Create_FromInvalidSwapResult_MapsFailureStateWithoutMutatingSnapshots()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));
            
            var move = new Move(new Pos(0, 0), new Pos(1, 1));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            var presentation = TurnPresentationFactory.Create(turnResult);
            
            Assert.IsNotNull(presentation);
            Assert.IsNotNull(presentation.InitialBoard);
            Assert.IsNotNull(presentation.FinalBoard);
            Assert.IsFalse(presentation.WasApplied);
            Assert.IsFalse(presentation.WasShuffled);
            Assert.IsFalse(presentation.CreatedMatch);
            Assert.AreEqual(0, presentation.TotalClearedCells);
            
            Assert.IsNull(presentation.SwapBoard);
            Assert.IsNull(presentation.DirectActivationBoard);
            Assert.IsNull(presentation.PrimaryResolveBoard);
            Assert.IsNull(presentation.GravityBoard);
            Assert.IsNull(presentation.RefillBoard);
            Assert.IsNull(presentation.ShuffleBoard);
            Assert.IsNotNull(presentation.CascadeBoards);
            Assert.AreEqual(0, presentation.CascadeBoards.Count);
            Assert.AreEqual(presentation.InitialBoard.ToDebugString(), presentation.FinalBoard.ToDebugString());
        }

        [Test]
        public void Create_FromNormalMatchResult_MapsCorePhaseSnapshots()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));
            
            var move = new Move(new Pos(2, 1), new Pos(2, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            var presentation = TurnPresentationFactory.Create(turnResult);
            
            Assert.IsTrue(presentation.WasApplied);
            Assert.IsTrue(presentation.CreatedMatch);
            Assert.AreNotEqual(0, presentation.TotalClearedCells);
            
            Assert.IsNotNull(presentation.SwapBoard);
            Assert.IsNotNull(presentation.PrimaryResolveBoard);
            Assert.IsNotNull(presentation.GravityBoard);
            Assert.IsNotNull(presentation.RefillBoard);
            Assert.AreNotEqual(presentation.InitialBoard.ToDebugString(), presentation.FinalBoard.ToDebugString());
        }

        [Test]
        public void Create_FromDirectSpecialActivationResult_MapsDirectActivationSnapshotWithoutSwapSnapshot()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));
            
            var move = new Move(new Pos(1, 1), new Pos(1, 2));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            var presentation = TurnPresentationFactory.Create(turnResult);
            
            Assert.IsTrue(presentation.WasApplied);
            Assert.IsFalse(presentation.CreatedMatch);
            
            Assert.IsNull(presentation.SwapBoard);
            Assert.IsNull(presentation.PrimaryResolveBoard);
            Assert.IsNotNull(presentation.DirectActivationBoard);
            Assert.IsNotNull(presentation.GravityBoard);
            Assert.IsNotNull(presentation.RefillBoard);
        }

        [Test]
        public void Create_FromShuffledResult_MapsShuffleSnapshot()
        {
            var board = new BoardModel(3, 3);

            board.Set(0, 0, Piece.ColorBomb());
            board.Set(0, 1, Piece.ColorBomb());
            board.Set(0, 2, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Blue));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));

            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.Normal(PieceColor.Blue));
            
            var move = new Move(new Pos(0, 0), new Pos(0, 1));
            var provider = new SequenceRandomProvider(new[] { 0, 0, 1, 0, 0, 1, 1, 3, 3, 0, 0, 0, 0, 0, 0, 0, 1 });
            var turnResult = TurnProcessor.TryProcessMove(board, move, provider);
            var presentation = TurnPresentationFactory.Create(turnResult);
            
            
            Assert.IsTrue(presentation.WasApplied);
            Assert.IsTrue(presentation.WasShuffled);
            
            Assert.IsNull(presentation.SwapBoard);
            Assert.IsNotNull(presentation.DirectActivationBoard);
            Assert.IsNotNull(presentation.ShuffleBoard);
            Assert.AreEqual(presentation.ShuffleBoard.ToDebugString(), presentation.FinalBoard.ToDebugString());
        }

        [Test]
        public void Create_FromCascadeResult_MapsCascadeSnapshotsInOrder()
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
            var presentation = TurnPresentationFactory.Create(turnResult);
            
            Assert.IsNotNull(presentation.CascadeBoards);
            Assert.AreEqual(turnResult.CascadeCount, presentation.CascadeBoards.Count);

            var cascadePhases = TurnTraceQueries.GetCascadePhases(turnResult.Trace);
            for(int i = 0; i < turnResult.CascadeCount; i++)
                Assert.AreEqual(cascadePhases[i].BoardAfterPhase.ToDebugString(), presentation.CascadeBoards[i].ToDebugString());
        }
    }
}