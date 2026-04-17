using System.Collections;
using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private TurnPlaybackController turnPlaybackController;
        [SerializeField] private int startHeight = 8;
        [SerializeField] private int startWidth = 8;
        private GameSession session;
        private BoardInputController inputController;
        private HintController hintController;

        public void Start()
        {
            session = new GameSession();
            hintController = new HintController(session, boardRenderer);
            var initialSnapshot = session.StartNewGame(startHeight, startWidth);
            boardRenderer.Render(initialSnapshot);

            inputController = new BoardInputController();
            inputController.CanAcceptInput = () => !turnPlaybackController.IsPlaying;
            boardRenderer.CellClicked += HandleBoardCellClicked;
            boardRenderer.CellClicked += inputController.OnCellClicked;
            inputController.MoveSubmitted += ApplyMove;
            inputController.SelectionChanged += boardRenderer.SetSelection;

            if (turnPlaybackController.IsPlaying)
                return;
            hintController.ShowHint();
        }
        private void ApplyMove(Move move)
        {
            hintController.ClearHint();
            var presentation = session.TryApplyMove(move);
            turnPlaybackController.Play(presentation);
        }
        private void HandleBoardCellClicked(int row, int col)
        {
            hintController.ClearHint();
        }
    }
}