using Core.Contracts;
using Core.Generation;
using Core.Matching;
using Core.Presentation;
using Core.Turns;

namespace Match3.Unity
{
    public class GameSession
    {
        public BoardModel CurrentBoard { get; private set; }
        public TurnResult LastTurnResult { get; private set; }
        public TurnPresentationModel LastPresentation { get; private set; }

        public BoardSnapshot StartNewGame(int height, int width)
        {
            CurrentBoard = BoardGenerator.Generate(height, width);
            LastTurnResult = null;
            LastPresentation = null;
            return BoardSnapshotFactory.Capture(CurrentBoard);
        }

        public TurnPresentationModel TryApplyMove(Move move)
        {
           var result = TurnProcessor.TryProcessMove(CurrentBoard, move);
           var presentation = TurnPresentationFactory.Create(result);
           
           LastTurnResult = result;
           LastPresentation = presentation;
           return presentation;
        }
        
        public BoardSnapshot StartDebugGame()
        {
            var board = new BoardModel(3, 4);

            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));

            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Green));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));

            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));

            CurrentBoard = board;
            LastTurnResult = null;
            LastPresentation = null;

            return BoardSnapshotFactory.Capture(CurrentBoard);
        }
    }
}