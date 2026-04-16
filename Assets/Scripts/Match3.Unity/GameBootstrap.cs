using System.Collections;
using Core.Matching;
using UnityEngine;

namespace Match3.Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private int startHeight = 8;
        [SerializeField] private int startWidth = 8;
        private GameSession session;
        private BoardInputController inputController;
        

        public void Start()
        {
            session = new GameSession();
            var initialSnapshot = session.StartNewGame(startHeight, startWidth);
            boardRenderer.Render(initialSnapshot);

            inputController = new BoardInputController();
            boardRenderer.CellClicked += inputController.OnCellClicked;
            inputController.MoveSubmitted += ApplyMove;
            inputController.SelectionChanged += boardRenderer.SetSelection;
        }

        private void ApplyMove(Move move)
        {
            var presentation = session.TryApplyMove(move);
            boardRenderer.Render(presentation.FinalBoard);
        }
    }
}