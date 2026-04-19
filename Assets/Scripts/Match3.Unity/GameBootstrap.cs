using System.Collections;
using Core.Matching;
using Core.Progress;
using Match3.Unity.GameMode;
using Match3.Unity.Save;
using TMPro;
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
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private float totalGameSeconds = 120f;
        [SerializeField] private GameOverPanel gameOverPanel;
       
        private GameSession session;
        private BoardInputController inputController;
        
        private HintController hintController;
        private IdleTracker idleTracker;
        private int consecutiveHintsShown;
        private int totalHintsShown;
        private bool hintShownForCurrentIdle;

        private IProgressStore progressStore;
        private int turnsPlayed;
        
        private TimerController timerController;

        private bool gameOverShown;
        

        public void Start()
        {
            progressStore = new JsonProgressStore();
            var progress = progressStore.Load(); 
            consecutiveHintsShown = 0;
            totalHintsShown = progress.AdaptiveHints.TotalHintsShown;
            turnsPlayed = progress.TurnsPlayed;
            
            session = new GameSession();
            var initialSnapshot = session.StartNewGame(startHeight, startWidth);
            boardRenderer.Render(initialSnapshot);
            
            timerController = new TimerController();
            timerController.StartTimer(totalGameSeconds);
            UpdateTimerText();
            
            inputController = new BoardInputController();
            inputController.CanAcceptInput = () => 
                !turnPlaybackController.IsPlaying && !timerController.IsExpired;
            
            idleTracker = new IdleTracker();
            hintController = new HintController(session, boardRenderer);
            
            boardRenderer.CellClicked += HandleBoardCellClicked;
            boardRenderer.CellClicked += inputController.OnCellClicked;
            inputController.MoveSubmitted += ApplyMove;
            inputController.SelectionChanged += boardRenderer.SetSelection;
            
            gameOverPanel.Hide();
            gameOverShown = false;
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
            if (!timerController.IsExpired)
            {
                timerController.Tick(Time.deltaTime);
                UpdateTimerText();
            }

            if (timerController.IsExpired)
            {
                if (!gameOverShown)
                {
                    hintController.ClearHint();
                    inputController.ClearSelection();
                    gameOverPanel.Show("Time is up.");
                    gameOverShown = true;
                }
                return;
            }
            
            if (turnPlaybackController.IsPlaying) return;
            
            idleTracker.Tick(Time.deltaTime);
            TryShowIdleHint();
        }

        private void UpdateTimerText()
        {
            var totalSeconds = Mathf.CeilToInt(timerController.RemainingSeconds);
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private void TryShowIdleHint()
        {
            if(timerController.IsExpired) return;
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
            if(timerController.IsExpired) return;
            if (turnPlaybackController.IsPlaying) return;
            var effectiveIdleSeconds = Mathf.Max(idleTracker.IdleSeconds, idleHintDelaySeconds);
            ShowAdaptiveHintInternal(effectiveIdleSeconds);
            hintShownForCurrentIdle = false;
        }

        private void ShowAdaptiveHintInternal(float idleSeconds)
        {
            if(timerController.IsExpired) return;
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
        
        public void OnRestartButtonClick()
        {
            StartNewGameFlow();
        }
        
        private void StartNewGameFlow()
        {
            hintController.ClearHint();
            inputController.ClearSelection();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
            consecutiveHintsShown = 0;
            
            var snapshot = session.StartNewGame(startHeight, startWidth);
            boardRenderer.Render(snapshot);
            
            timerController.StartTimer(totalGameSeconds);
            UpdateTimerText();
            
            gameOverPanel.Hide();
            gameOverShown = false;
        }

        
    }
}