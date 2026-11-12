using Core.Board;
using Core.Hint;
using Core.Progress;
using Match3.Unity.Board;
using Match3.Unity.GameFlow;

namespace Match3.Unity.Hint
{
    public class HintController
    {
        private readonly GameSession _gameSession;
        private readonly BoardRenderer _boardRenderer;
        private readonly HintExplanationView _hintExplanation;

        public HintController(GameSession gameSession, BoardRenderer boardRenderer,  HintExplanationView hintExplanation)
        {
            _gameSession = gameSession;
            _boardRenderer = boardRenderer;
            _hintExplanation = hintExplanation;
        }

        public Move? ShowAdaptiveHint(float idleSeconds, AdaptiveHintState state, HintGoalContext goalContext)
        {
            var hint = _gameSession.GetAdaptiveHint(idleSeconds, state,  goalContext);
            if (!hint.Move.HasValue)
            {
                ClearHint();
                return null;
            }
            
            _boardRenderer.SetHint(hint.Move.Value);
            _hintExplanation.Show(HintExplanationFormatter.Format(hint.Explanation));
            return hint.Move.Value;
        }

        public void ClearHint()
        {
            _boardRenderer.SetHint(null);
            _hintExplanation.Hide();
        }
    }
}