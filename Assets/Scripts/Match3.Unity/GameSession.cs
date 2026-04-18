using Core.Contracts;
using Core.Generation;
using Core.Hint;
using Core.Matching;
using Core.Presentation;
using Core.Progress;
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

        public HintResult GetAdaptiveHint(float idleSeconds, AdaptiveHintState state)
        {
            var context = HintContextFactory.Create(idleSeconds, state);
            return HintFinder.FindAdaptiveHint(CurrentBoard, context);
        }
    }
}