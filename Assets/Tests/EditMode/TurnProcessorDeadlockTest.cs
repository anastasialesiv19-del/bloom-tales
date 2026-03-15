using Core.Board;
using Core.Generation;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class TurnProcessorDeadlockTest
    {
        [Test][Ignore("сценарій WasShuffled == true потребує контрольованого refill з поточним random цей тест крихкий.")]
        public void TryProcessMove_ShufflesBoard_WhenSuccessfulMoveLeavesDeadlock()
        {
            
        }

        [Test]
        public void TryProcessMove_DoesNotShuffle_WhenBoardRemainsPlayable()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, PieceType.Yellow);
            board.Set(0, 1, PieceType.Blue);
            board.Set(0, 2, PieceType.Green);
            board.Set(0, 3, PieceType.Red);

            board.Set(1, 0, PieceType.Green);
            board.Set(1, 1, PieceType.Green);
            board.Set(1, 2, PieceType.Red);
            board.Set(1, 3, PieceType.Green);

            board.Set(2, 0, PieceType.Yellow);
            board.Set(2, 1, PieceType.Blue);
            board.Set(2, 2, PieceType.Green);
            board.Set(2, 3, PieceType.Red);
            
            var move = new Move(new Pos(1,2), new Pos(1,3));
            var turnResult = TurnProcessor.TryProcessMove(board, move);
            
            Assert.IsTrue(BoardShuffler.IsPlayableBoard(board));
            Assert.IsTrue(turnResult.WasSwapValid);
            Assert.IsTrue(turnResult.WasApplied);
            Assert.IsFalse(turnResult.WasShuffled);
            Assert.IsTrue(turnResult.CreatedMatch);
            Assert.AreEqual(FailureReason.None, turnResult.FailureReason);
        }        
    }
}