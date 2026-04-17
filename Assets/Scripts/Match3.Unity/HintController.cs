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

        public void ShowHint()
        {
            var hint = gameSession.GetBestHint();
            if (hint.Move == null)
            {
                boardRenderer.SetHint(null);
                return;
            }
            boardRenderer.SetHint(hint.Move.Value);
        }

        public void ClearHint()
        {
            boardRenderer.SetHint(null);
        }
    }
}