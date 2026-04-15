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
        
        [SerializeField] private bool useDebugBoard = true;

        public void Start()
        {
            session = new GameSession();
            var initialSnapshot = useDebugBoard
                ? session.StartDebugGame()
                : session.StartNewGame(startHeight, startWidth);
            boardRenderer.Render(initialSnapshot);

            ApplyTestMoveWithDelay();
        }
        
        public void ApplyTestMoveWithDelay()
        {
            StartCoroutine(ApplyTestMoveAfterDelay());
        }
        
        private IEnumerator ApplyTestMoveAfterDelay()
        {
            yield return new WaitForSeconds(5);

            var move = new Move(new Pos(1, 2), new Pos(1, 3));
            var presentation = session.TryApplyMove(move);
            boardRenderer.Render(presentation.FinalBoard);
        }

        public void ApplyTestMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            var move = new Move(new Pos(fromRow, fromCol), new Pos(toRow, toCol));
            var presentation = session.TryApplyMove(move);
            boardRenderer.Render(presentation.FinalBoard);
        }
    }
}