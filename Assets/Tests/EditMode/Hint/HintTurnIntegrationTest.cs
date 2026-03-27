using System.Linq;
using Core.Hint;
using Core.Matching;
using Core.Turns;
using NUnit.Framework;

namespace Tests.EditMode.Hint
{
    public class HintTurnIntegrationTest
    {
        [Test]
        public void FindBestHint_WhenExplanationContainsCreatesMatch_ExecutingMoveProducesAppliedTurn()
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
            
            var hint = HintFinder.FindBestHint(board);
            
            Assert.IsNotNull(hint);
            Assert.IsNotNull(hint.Explanation);
            Assert.IsTrue(hint.Explanation.Reasons.Contains(HintReasonType.CreatesMatch));

            var boardCopy = CloneBoard(board);
            var turn = TurnProcessor.TryProcessMove(boardCopy, hint.Move.Value);
            
            Assert.IsTrue(turn.WasApplied);
            Assert.AreEqual(FailureReason.None, turn.FailureReason);
            Assert.Greater(turn.TotalClearedCells, 0);
        }
        
        [Test]
        public void FindBestHint_WhenExplanationContainsCreatesSpecial_ExecutingMoveSpawnsSpecialInPrimaryResolution()
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
            
            var hint = HintFinder.FindBestHint(board);
            
            var boardCopy = CloneBoard(board);
            var turn = TurnProcessor.TryProcessMove(boardCopy, hint.Move.Value);
            
            var primaryPhase = turn.Trace.Phases
                .Single(p => p.Type == TurnPhaseType.PrimaryMatchResolved);
            
            Assert.IsNotNull(hint.Move);
            Assert.IsNotNull(hint.Explanation);
            Assert.IsTrue(hint.Explanation.Reasons.Contains(HintReasonType.CreatesSpecial));
            
            Assert.IsTrue(turn.WasApplied);
            Assert.AreEqual(FailureReason.None, turn.FailureReason);
            Assert.AreEqual(TurnPhaseType.PrimaryMatchResolved, primaryPhase.Type);
            Assert.Greater(primaryPhase.SpawnedSpecials, 0);
        }
        
        [Test]
        public void FindBestHint_WhenExplanationContainsActivatesSpecialDirectly_ExecutingMoveUsesDirectActivationPath()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.RowClear(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 3, Piece.ColumnClear(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            var hint = HintFinder.FindBestHint(board);
            
            var boardCopy = CloneBoard(board);
            var turn = TurnProcessor.TryProcessMove(boardCopy, hint.Move.Value);
            
            Assert.IsNotNull(hint.Move);
            Assert.IsNotNull(hint.Explanation);
            Assert.IsTrue(hint.Explanation.Reasons.Contains(HintReasonType.ActivatesSpecialDirectly));
            
            Assert.IsTrue(turn.WasApplied);
            Assert.IsFalse(turn.CreatedMatch);
            Assert.AreEqual(FailureReason.None, turn.FailureReason);
            Assert.IsTrue(turn.Trace.Phases.Any(p => p.Type == TurnPhaseType.DirectSpecialActivation));
        }

        [Test]
        public void FindAdaptiveHint_DoesNotChangeSelectedMoveComparedToBestHint()
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

            var context = new HintContext(5, 0);
            var bestHint = HintFinder.FindBestHint(board);
            var adaptiveHint = HintFinder.FindAdaptiveHint(board, context);

            Assert.IsNotNull(bestHint.Move);
            Assert.IsNotNull(bestHint.Explanation);

            Assert.IsNotNull(adaptiveHint.Move);
            Assert.IsNotNull(adaptiveHint.Explanation);

            Assert.AreEqual(bestHint.Move.Value, adaptiveHint.Move.Value);
            Assert.AreEqual(bestHint.Explanation.Score, adaptiveHint.Explanation.Score);
            Assert.AreNotEqual(HintStrength.None, adaptiveHint.Strength);
            Assert.IsTrue(adaptiveHint.Strength != bestHint.Strength);
        }
        
        private static BoardModel CloneBoard(BoardModel source)
        {
            var clone = new BoardModel(source.Height, source.Width);

            for (int row = 0; row < source.Height; row++)
            {
                for (int col = 0; col < source.Width; col++)
                {
                    clone.Set(row, col, source.Get(row, col));
                }
            }

            return clone;
        }

    }
}