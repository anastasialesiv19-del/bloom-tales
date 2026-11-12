using Core.Board;
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
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
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
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
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
        public void Create_FromLineOrAreaWithNormalResult_MapsSwapAndDirectActivationSnapshot()
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
            var turnResult = TurnProcessor.TryProcessMove(board, move, new SeededRandomProvider());
            var presentation = TurnPresentationFactory.Create(turnResult);

            Assert.IsTrue(presentation.WasApplied);
            Assert.IsFalse(presentation.CreatedMatch);
            Assert.IsNotNull(presentation.SwapBoard);

            Assert.AreEqual(Piece.Normal(PieceColor.Yellow), presentation.SwapBoard.Get(1, 1));
            Assert.AreEqual(Piece.RowClear(PieceColor.Blue), presentation.SwapBoard.Get(1, 2));

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
            var result = CascadeTestScenario.ResolveTwoWaves();
            
            Assert.IsTrue(result.WasApplied);
            Assert.AreEqual(2, result.CascadeCount);
            
            var presentation = TurnPresentationFactory.Create(result);
            var phases = TurnTraceQueries.GetCascadePhases(result.Trace);
            
            Assert.AreEqual(2, presentation.CascadeBoards.Count);
            Assert.AreNotEqual(phases[0].BoardAfterPhase.ToDebugString(), phases[1].BoardAfterPhase.ToDebugString());

            for (var i = 0; i < 2; i++)
            {
                Assert.AreEqual(phases[i].BoardAfterPhase.ToDebugString(), presentation.CascadeBoards[i].ToDebugString());
            }
        }
    }
}
