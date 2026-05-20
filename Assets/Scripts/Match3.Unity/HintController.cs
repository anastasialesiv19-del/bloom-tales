using Core.Hint;
using Core.Matching;
using Core.Progress;

namespace Match3.Unity
{
    public class HintController
    {
        private readonly GameSession gameSession;
        private readonly BoardRenderer boardRenderer;

        public HintController(GameSession gameSession, BoardRenderer boardRenderer)
        {
            this.gameSession = gameSession;
            this.boardRenderer = boardRenderer;
        }

        public Move? ShowAdaptiveHint(float idleSeconds, AdaptiveHintState state, HintGoalContext goalContext)
        {
            var hint = gameSession.GetAdaptiveHint(idleSeconds, state,  goalContext);
            if (!hint.Move.HasValue)
            {
                boardRenderer.SetHint(null);
                return null;
            }
            boardRenderer.SetHint(hint.Move.Value);
            return hint.Move.Value;
        }

        public void ClearHint()
        {
            boardRenderer.SetHint(null);
        }
    }
}