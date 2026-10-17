using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Matching;
using Core.Progress;
using Match3.Unity.GameFlow;
using Match3.Unity.GameMode;
using Match3.Unity.Goals;
using Match3.Unity.Menu;
using Match3.Unity.Save;
using Match3.Unity.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Match3.Unity
{
    public partial class GameBootstrap : MonoBehaviour
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
        [SerializeField] private Sprite level1Background;
        [SerializeField] private Sprite level2Background;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private PausePanel pausePanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject defeatPanel;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Transform goalsRoot;
        [SerializeField] private GoalItemView goalItemPrefab;
        [SerializeField] private Sprite boxGoalIcon;
        [SerializeField] private Sprite scoreGoalIcon;
        [SerializeField] private PieceSpriteLibrary pieceSpriteLibrary ;
        
        private GameSession session;
        private BoardInputController inputController;
        private LevelId currentLevelId;
        private HintController hintController;
        private IdleTracker idleTracker;
        private int consecutiveHintsShown;
        private int totalHintsShown;
        private bool hintShownForCurrentIdle;

        private IProgressStore progressStore;
        private int turnsPlayed;
        
        private LevelResultStore levelResultStore;
        private Move? activeHintMove;
        private int manualHintsShownThisRun;
        private int idleHintsShownThisRun;
        private int hintsAppliedThisRun;
        private int clearedCellsThisRun;
        private int destroyedObstaclesThisRun;
        private int scoreThisRun;
        private int turnsUsedThisRun;
        private bool resultSavedForCurrentRun;
        
        private TimerController timerController;
        private MoveLimitController moveLimitController;

        private bool isPaused;
        private bool gameOverShown;
        
        private List<IGoal> goals;
        private List<GoalItemView> goalViews = new();
        private IReadOnlyList<ObstacleDefinition> currentObstacles;
        private IReadOnlyList<GoalDefinition> currentGoalDefinitions;
         

        public void Start()
        {
            ApplySelectedLevelConfigIfPresent();
            var theme = LevelThemeResolver.Resolve(currentLevelId, level1Background, level2Background);
            ApplyTheme(theme);
            progressStore = new JsonProgressStore();
            var progress = progressStore.Load(); 
            levelResultStore = new LevelResultStore();
            ResetRunStats();
            consecutiveHintsShown = 0;
            totalHintsShown = progress.AdaptiveHints.TotalHintsShown;
            turnsPlayed = progress.TurnsPlayed;
            
            session = new GameSession();
            var initialSnapshot = session.StartNewGame(startHeight, startWidth, currentObstacles);
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
                !turnPlaybackController.IsPlaying && !IsCurrentModeExpired() &&  !isPaused && !gameOverShown;
            
            idleTracker = new IdleTracker();
            hintController = new HintController(session, boardRenderer);
            
            boardRenderer.CellClicked += HandleBoardCellClicked;
            boardRenderer.CellClicked += inputController.OnCellClicked;
            inputController.MoveSubmitted += ApplyMove;
            inputController.SelectionChanged += boardRenderer.SetSelection;
            
            pausePanel.Hide();
            isPaused = false;
            
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
            
            RebuildGoals();
            RebuildGoalViews();
            RefreshGoalHud();
        }
        
        private void Update()
        {
            if (isPaused) return;
            
            if (gameMode == GameModeType.Timer && !timerController.IsExpired)
            {
                timerController.Tick(Time.deltaTime);
                RefreshResourceText();
            }
            if (turnPlaybackController.IsPlaying) return;
            
            if (!gameOverShown &&  goals.All(g => g.IsCompleted))
            {
                HandleGameResult(GameResultType.Victory);
                gameOverShown = true;
                return;
            }
            
            if (IsCurrentModeExpired())
            {
                if (!gameOverShown)
                {
                    var result = GameResultResolver.Resolve(
                        gameMode,
                        true);
                    HandleGameResult(result);
                    gameOverShown = true;
                }
                return;
            }
            idleTracker.Tick(Time.deltaTime);
            TryShowIdleHint();
        }
        
        private void ApplyMove(Move move)
        {
            var activeHintWasApplied = activeHintMove.HasValue && IsSameMove(activeHintMove.Value, move);

            activeHintMove = null;
            hintController.ClearHint();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
            consecutiveHintsShown = 0;

            var presentation = session.TryApplyMove(move);
            if (presentation.WasApplied)
            {
                if (activeHintWasApplied)
                    hintsAppliedThisRun++;

                foreach (var goal in goals)
                    goal.Apply(presentation.ClearStats);
                
                scoreThisRun += presentation.ClearStats.TotalClearedCells;
                RefreshGoalHud();

                turnsPlayed++;
                turnsUsedThisRun++;

                clearedCellsThisRun += presentation.ClearStats.TotalClearedCells;
                destroyedObstaclesThisRun += presentation.ClearStats.DestroyedObstacles;
                
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
        
        private static bool IsSameMove(Move a, Move b)
        {
            return
                a.From.Equals(b.From) && a.To.Equals(b.To) ||
                a.From.Equals(b.To) && a.To.Equals(b.From);
        }
        
        private void SaveProgress()
        {
            var progress = new PlayerProgress(new AdaptiveHintState(0, totalHintsShown), turnsPlayed);
            progressStore.Save(progress); 
        }
    }
}