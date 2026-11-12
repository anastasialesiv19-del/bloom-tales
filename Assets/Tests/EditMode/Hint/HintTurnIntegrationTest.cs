using Tests.EditMode.TestDoubles;
using System.Linq;
using Core.Board;
using Core.Hint;
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
            var turn = TurnProcessor.TryProcessMove(boardCopy, hint.Move.Value, new SeededRandomProvider());

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
            var turn = TurnProcessor.TryProcessMove(boardCopy, hint.Move.Value, new SeededRandomProvider());

            var primaryPhase = turn.Trace.Phases.Single(p => p.Type == TurnPhaseType.PrimaryMatchResolved);

            Assert.IsNotNull(hint.Move);
            Assert.IsNotNull(hint.Explanation);
            Assert.IsTrue(hint.Explanation.Reasons.Contains(HintReasonType.CreatesSpecial));

            Assert.IsTrue(turn.WasApplied);
            Assert.AreEqual(FailureReason.None, turn.FailureReason);
            Assert.AreEqual(TurnPhaseType.PrimaryMatchResolved, primaryPhase.Type);
            Assert.Greater(primaryPhase.SpawnedSpecials, 0);
        }

        [Test]
        public void FindBestHint_SelectedMoveExplanation_MatchesExecutionPath()
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

            Assert.IsNotNull(hint.Move);
            Assert.IsNotNull(hint.Explanation);

            var turn = TurnProcessor.TryProcessMove(boardCopy, hint.Move.Value, new SeededRandomProvider());

            Assert.IsTrue(turn.WasApplied);
            Assert.AreEqual(FailureReason.None, turn.FailureReason);

            var predictsDirectActivation = hint.Explanation.Reasons.Contains(HintReasonType.ActivatesSpecialDirectly);
            var executedDirectActivation = turn.Trace.Phases.Any(phase => phase.Type == TurnPhaseType.DirectSpecialActivation);

            Assert.AreEqual(predictsDirectActivation, executedDirectActivation, "Hint explanation must match the actual execution path.");
        }

        [Test]
        public void DirectSpecialMove_ExecutesDirectActivationPath()
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

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var outcome = HintMoveOutcomeEstimator.Estimate(board, move);

            Assert.AreEqual(1, outcome.ClearedByColor.Count);
            Assert.AreEqual(3, outcome.ClearedByColor[PieceColor.Green]);
            Assert.AreEqual(0, outcome.EstimatedObstacleDamage);

            var boardCopy = CloneBoard(board);
            var turn = TurnProcessor.TryProcessMove(boardCopy, move, new SeededRandomProvider());

            Assert.IsTrue(turn.WasApplied);
            Assert.IsFalse(turn.CreatedMatch);
            Assert.AreEqual(FailureReason.None, turn.FailureReason);
            Assert.IsTrue(turn.Trace.Phases.Any(phase => phase.Type == TurnPhaseType.DirectSpecialActivation));
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

        [Test]
        public void ColorBombWithNormal_EstimateMatchesPrimaryEffect_AndDoesNotChangeBoard()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Blue));
            board.Set(0, 3, Piece.Normal(PieceColor.Green));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.ColorBomb());
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 2), new Pos(2, 2));
            var before = CloneBoard(board);
            var outcome = HintMoveOutcomeEstimator.Estimate(board, move);

            Assert.AreEqual(1, outcome.ClearedByColor.Count);
            Assert.AreEqual(4, outcome.ClearedByColor[PieceColor.Green]);
            Assert.AreEqual(0, outcome.EstimatedObstacleDamage);

            for (var row = 0; row < board.Height; row++)
            {
                for (var col = 0; col < board.Width; col++)
                {
                    Assert.AreEqual(before.Get(row, col), board.Get(row, col), $"Estimate changed cell ({row}, {col}).");
                }
            }

            var turn = TurnProcessor.TryProcessMove(CloneBoard(before), move, new SeededRandomProvider());
            var directPhase = turn.Trace.Phases.Single(phase => phase.Type == TurnPhaseType.DirectSpecialActivation);
            var after = directPhase.BoardAfterPhase;
            
            Assert.IsNotNull(after);

            var clearedGreen = 0;
            for (var row = 0; row < before.Height; row++)
            {
                for (var col = 0; col < before.Width; col++)
                {
                    if (before.Get(row, col).Color == PieceColor.Green && after.Get(row, col).IsEmpty)
                    {
                        clearedGreen++;
                    }
                }
            }

            Assert.AreEqual(outcome.ClearedByColor[PieceColor.Green], clearedGreen, "Estimated green clears must match the primary effect.");
        }

        [Test]
        public void Estimate_RowClearWithRowClear_WhenThirdSpecialInZone_MatchesPrimaryEffect()
        {
            var board = new BoardModel(4, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Yellow));

            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.RowClear(PieceColor.Red));
            board.Set(1, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 3, Piece.ColumnClear(PieceColor.Blue));

            board.Set(2, 0, Piece.Normal(PieceColor.Green));
            board.Set(2, 1, Piece.RowClear(PieceColor.Yellow));
            board.Set(2, 2, Piece.Normal(PieceColor.Red));
            board.Set(2, 3, Piece.Normal(PieceColor.Blue));

            board.Set(3, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(3, 1, Piece.Normal(PieceColor.Green));
            board.Set(3, 2, Piece.Normal(PieceColor.Blue));
            board.Set(3, 3, Piece.Normal(PieceColor.Red));

            var move = new Move(new Pos(1, 1), new Pos(2, 1));
            var outcome = HintMoveOutcomeEstimator.Estimate(board, move);
            var turn = TurnProcessor.TryProcessMove(CloneBoard(board), move, new SeededRandomProvider());

            Assert.IsTrue(turn.WasApplied);
            Assert.IsFalse(turn.CreatedMatch);

            var directPhase = turn.Trace.Phases.Single(phase => phase.Type == TurnPhaseType.DirectSpecialActivation);

            Assert.IsNotNull(directPhase.BoardAfterPhase);

            for (var row = 0; row < board.Height; row++)
            {
                for (var col = 0; col < board.Width; col++)
                {
                    if (row == 1 || row == 2 || col == 3)
                    {
                        Assert.IsTrue(directPhase.BoardAfterPhase.Get(row, col).IsEmpty);
                    }
                    else
                    {
                        Assert.AreEqual(directPhase.BoardAfterPhase.Get(row, col), turn.InitialBoardSnapshot.Get(row, col));
                    }
                }
            }
            
            Assert.AreEqual(4, outcome.ClearedByColor.Count);
            Assert.AreEqual(1, outcome.ClearedByColor[PieceColor.Green]);
            Assert.AreEqual(3, outcome.ClearedByColor[PieceColor.Yellow]);
            Assert.AreEqual(3, outcome.ClearedByColor[PieceColor.Blue]);
            Assert.AreEqual(3, outcome.ClearedByColor[PieceColor.Red]);
            Assert.AreEqual(0, outcome.EstimatedObstacleDamage);
        }

        private static BoardModel CloneBoard(BoardModel source)
        {
            var clone = new BoardModel(source.Height, source.Width);

            for (var row = 0; row < source.Height; row++)
            {
                for (var col = 0; col < source.Width; col++)
                {
                    clone.Set(row, col, source.Get(row, col));
                }
            }
            return clone;
        }
    }
}
