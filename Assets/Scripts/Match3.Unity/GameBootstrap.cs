using System.Collections;
using Core.Contracts;
using Core.Matching;
using Core.Progress;
using Match3.Unity.GameMode;
using Match3.Unity.Menu;
using Match3.Unity.Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private TurnPlaybackController turnPlaybackController;
        [SerializeField] private int startHeight = 8;
        [SerializeField] private int startWidth = 8;
        [SerializeField] private float idleHintDelaySeconds = 6f;
        [SerializeField] private GameModeType gameMode = GameModeType.Timer;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private float totalGameSeconds = 120f;
        [SerializeField] private int totalMoves = 20;
        [SerializeField] private PausePanel pausePanel;
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
        private MoveLimitController moveLimitController;

        private bool isPaused;
        private bool gameOverShown;
        

        public void Start()
        {
            ApplySelectedLevelConfigIfPresent();
            progressStore = new JsonProgressStore();
            var progress = progressStore.Load(); 
            consecutiveHintsShown = 0;
            totalHintsShown = progress.AdaptiveHints.TotalHintsShown;
            turnsPlayed = progress.TurnsPlayed;
            
            session = new GameSession();
            var initialSnapshot = session.StartNewGame(startHeight, startWidth);
            boardRenderer.Render(initialSnapshot);
            
            timerController = new TimerController();
            moveLimitController = new MoveLimitController();
            
            if (gameMode == GameModeType.Timer)
                timerController.StartTimer(totalGameSeconds);
            else
                moveLimitController.StartMoves(totalMoves);
            
            RefreshResourceText();
            
            inputController = new BoardInputController();
            inputController.CanAcceptInput = () => 
                !turnPlaybackController.IsPlaying && !IsCurrentModeExpired() &&  !isPaused;
            
            idleTracker = new IdleTracker();
            hintController = new HintController(session, boardRenderer);
            
            boardRenderer.CellClicked += HandleBoardCellClicked;
            boardRenderer.CellClicked += inputController.OnCellClicked;
            inputController.MoveSubmitted += ApplyMove;
            inputController.SelectionChanged += boardRenderer.SetSelection;
            
            pausePanel.Hide();
            isPaused = false;
            
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
                
                if (gameMode == GameModeType.Moves)
                {
                    moveLimitController.ConsumeMove();
                    RefreshResourceText();
                }
                
                SaveProgress();
            }

            turnPlaybackController.Play(presentation);
        }
        
        private void Update()
        {
            if (isPaused)
                return;
            
            if (gameMode == GameModeType.Timer && !timerController.IsExpired)
            {
                timerController.Tick(Time.deltaTime);
                RefreshResourceText();
            }
            if (turnPlaybackController.IsPlaying) return;
            if (IsCurrentModeExpired())
            {
                if (!gameOverShown)
                {
                    hintController.ClearHint();
                    inputController.ClearSelection();
                    gameOverPanel.Show(GetGameOverMessage());
                    gameOverShown = true;
                }
                return;
            }
            
            idleTracker.Tick(Time.deltaTime);
            TryShowIdleHint();
        }

        private void RefreshResourceText()
        {

            if (gameMode == GameModeType.Timer)
            {
                var totalSeconds = Mathf.CeilToInt(timerController.RemainingSeconds);
                var minutes = totalSeconds / 60;
                var seconds = totalSeconds % 60;
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                timerText.text = $"Moves: {moveLimitController.RemainingMoves}";
            }
        }

        private bool IsCurrentModeExpired()
        {
            return gameMode == GameModeType.Timer ? timerController.IsExpired : moveLimitController.IsExpired;
        }

        private void TryShowIdleHint()
        {
            if (isPaused)
                return;
            if(IsCurrentModeExpired()) return;
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
            if (isPaused)
                return;
            if(IsCurrentModeExpired()) return;
            if (turnPlaybackController.IsPlaying) return;
            var effectiveIdleSeconds = Mathf.Max(idleTracker.IdleSeconds, idleHintDelaySeconds);
            ShowAdaptiveHintInternal(effectiveIdleSeconds);
            hintShownForCurrentIdle = false;
        }

        private void ShowAdaptiveHintInternal(float idleSeconds)
        {
            if (isPaused)
                return;
            if(IsCurrentModeExpired()) return;
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

        public void OnPauseButtonClick()
        {
            if (turnPlaybackController.IsPlaying)
                return;
            if (IsCurrentModeExpired())
                return;
            
            hintController.ClearHint();
            inputController.ClearSelection();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
            
            pausePanel.Show();
            isPaused = true;
        }

        public void OnResumeButtonClick()
        {
            pausePanel.Hide();
            isPaused = false;
        }

        public void OnBackToMenuClicked()
        {
            hintController.ClearHint();
            inputController.ClearSelection();
            pausePanel.Hide();
            isPaused = false;
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;

            SelectedLevelState.Current = null;
            SceneManager.LoadScene("MainMenuScene");
        }

        private void SaveProgress()
        {
            var progress = new PlayerProgress(new AdaptiveHintState(0, totalHintsShown), turnsPlayed);
            progressStore.Save(progress); 
        }
        
        public void OnRestartButtonClick()
        {
            if (isPaused)
            {
                pausePanel.Hide();
                isPaused = false;
            }
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
            boardRenderer.RebuildAndRender(snapshot);
            
            if (gameMode == GameModeType.Timer)
            {
                timerController.StartTimer(totalGameSeconds);
            }
            else
            {
                moveLimitController.ResetMoves(totalMoves);
            }

            RefreshResourceText();
            
            gameOverPanel.Hide();
            gameOverShown = false;
        }
        
        private string GetGameOverMessage()
        {
            return gameMode == GameModeType.Timer ? "Time is up" : "No moves left";
        }

        private void ApplySelectedLevelConfigIfPresent()
        {
            var config = SelectedLevelState.Current;
            if (config == null) return;
            
            gameMode = config.GameMode;
            totalMoves = config.TotalMoves;
            totalGameSeconds = config.TotalGameSeconds;

            SelectedLevelState.Current = null;
        }
    }
}