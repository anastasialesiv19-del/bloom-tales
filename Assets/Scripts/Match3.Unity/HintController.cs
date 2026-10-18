using Core.Hint;
using Core.Matching;
using Core.Progress;

namespace Match3.Unity
{
    public class HintController
    {
        private readonly GameSession gameSession;
        private readonly BoardRenderer boardRenderer;
        private readonly HintExplanationView hintExplanation;

        public HintController(GameSession gameSession, BoardRenderer boardRenderer,  HintExplanationView hintExplanation)
        {
            this.gameSession = gameSession;
            this.boardRenderer = boardRenderer;
            this.hintExplanation = hintExplanation;
        }

        public Move? ShowAdaptiveHint(float idleSeconds, AdaptiveHintState state, HintGoalContext goalContext)
        {
            var hint = gameSession.GetAdaptiveHint(idleSeconds, state,  goalContext);
            if (!hint.Move.HasValue)
            {
                ClearHint();
                return null;
            }
            boardRenderer.SetHint(hint.Move.Value);
            hintExplanation.Show(HintExplanationFormatter.Format(hint.Explanation));
            return hint.Move.Value;
        }

        public void ClearHint()
        {
            boardRenderer.SetHint(null);
            hintExplanation.Hide();
        }
    }
}