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

        public bool ShowAdaptiveHint(float idleSeconds, AdaptiveHintState state)
        {
            var hint = gameSession.GetAdaptiveHint(idleSeconds, state);
            if (hint.Move == null)
            {
                boardRenderer.SetHint(null);
                return false;
            }
            boardRenderer.SetHint(hint.Move.Value);
            return true;
        }

        public void ClearHint()
        {
            boardRenderer.SetHint(null);
        }
    }
}