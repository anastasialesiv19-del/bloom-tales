using System.Collections;
using Core.Matching;
using Core.Progress;
using Match3.Unity.Save;
using UnityEngine;

namespace Match3.Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private TurnPlaybackController turnPlaybackController;
        [SerializeField] private int startHeight = 8;
        [SerializeField] private int startWidth = 8;
        [SerializeField] private float idleHintDelaySeconds = 6f;
       
        private GameSession session;
        private BoardInputController inputController;
        
        private HintController hintController;
        private IdleTracker idleTracker;
        private int consecutiveHintsShown;
        private int totalHintsShown;
        private bool hintShownForCurrentIdle;

        private IProgressStore progressStore;
        private int turnsPlayed;
        

        public void Start()
        {
            progressStore = new JsonProgressStore();
            var progress = progressStore.Load(); 
            consecutiveHintsShown = 0;
            totalHintsShown = progress.AdaptiveHints.TotalHintsShown;
            turnsPlayed = progress.TurnsPlayed;
            
            session = new GameSession();
            idleTracker = new IdleTracker();
            
            var initialSnapshot = session.StartNewGame(startHeight, startWidth);
            boardRenderer.Render(initialSnapshot);

            inputController = new BoardInputController();
            inputController.CanAcceptInput = () => !turnPlaybackController.IsPlaying;
            
            hintController = new HintController(session, boardRenderer);
            
            boardRenderer.CellClicked += HandleBoardCellClicked;
            boardRenderer.CellClicked += inputController.OnCellClicked;
            inputController.MoveSubmitted += ApplyMove;
            inputController.SelectionChanged += boardRenderer.SetSelection;
        }

        private void ApplyMove(Move move)
        {
            hintController.ClearHint();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
            consecutiveHintsShown = 0;
            
            var presentation = session.TryApplyMove(move);
            if (presentation.WasApplied)
            {
                turnsPlayed++;
                consecutiveHintsShown = 0;
                SaveProgress();
            }
            turnPlaybackController.Play(presentation);
        }
        
        private void Update()
        {
            if (turnPlaybackController.IsPlaying)
                return;
            idleTracker.Tick(Time.deltaTime);
            TryShowIdleHint();
        }

        private void TryShowIdleHint()
        {
            if (hintShownForCurrentIdle) return;
            if (idleTracker.IdleSeconds < idleHintDelaySeconds) return;
            ShowAdaptiveHintInternal(idleTracker.IdleSeconds);
            hintShownForCurrentIdle = true;
        }
        
        private void HandleBoardCellClicked(int row, int col)
        {
            hintController.ClearHint();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
        }

        public void OnHintButtonClick()
        {
            if (turnPlaybackController.IsPlaying) return;
            var effectiveIdleSeconds = Mathf.Max(idleTracker.IdleSeconds, idleHintDelaySeconds);
            ShowAdaptiveHintInternal(effectiveIdleSeconds);
            hintShownForCurrentIdle = false;
        }

        private void ShowAdaptiveHintInternal(float idleSeconds)
        {
            hintController.ClearHint();
            
            var state = new AdaptiveHintState(consecutiveHintsShown, totalHintsShown);
            var wasShown = hintController.ShowAdaptiveHint(idleSeconds, state);
            idleTracker.ResetIdle();

            if (wasShown)
            {
                consecutiveHintsShown++;
                totalHintsShown++;
                SaveProgress();
            }
            else
                consecutiveHintsShown = 0;
        }

        private void SaveProgress()
        {
            var progress = new PlayerProgress(new AdaptiveHintState(0, totalHintsShown), turnsPlayed);
            progressStore.Save(progress); 
        }
    }
}