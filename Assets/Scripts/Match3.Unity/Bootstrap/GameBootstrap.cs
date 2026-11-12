using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Progress;
using Match3.Unity.Audio;
using Match3.Unity.Board;
using Match3.Unity.GameFlow;
using Match3.Unity.GameMode;
using Match3.Unity.Goals;
using Match3.Unity.Hint;
using Match3.Unity.Levels;
using Match3.Unity.Menu;
using Match3.Unity.Save;
using Match3.Unity.Settings;
using Match3.Unity.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Unity.Bootstrap
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
        [SerializeField] private Sprite level3Background;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private PausePanel pausePanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject defeatPanel;
        [SerializeField] private TMP_Text levelTitleText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Transform goalsRoot;
        [SerializeField] private GoalItemView goalItemPrefab;
        [SerializeField] private Sprite boxGoalIcon;
        [SerializeField] private Sprite scoreGoalIcon;
        [SerializeField] private PieceSpriteLibrary pieceSpriteLibrary; 
        [SerializeField] private HintExplanationView hintExplanationView;
        [SerializeField] private SettingsPanelView settingsPanelView;
        
        private GameSession _session;
        private BoardInputController _inputController;
        private LevelId _currentLevelId;
        private HintController _hintController;
        private IdleTracker _idleTracker;
        private int _consecutiveHintsShown;
        private int _totalHintsShown;
        private bool _hintShownForCurrentIdle;

        private IProgressStore _progressStore;
        private int _turnsPlayed;
        
        private LevelResultStore _levelResultStore;
        private Move? _activeHintMove;
        private int _manualHintsShownThisRun;
        private int _idleHintsShownThisRun;
        private int _hintsAppliedThisRun;
        private int _clearedCellsThisRun;
        private int _destroyedObstaclesThisRun;
        private int _scoreThisRun;
        private int _turnsUsedThisRun;
        private bool _resultSavedForCurrentRun;
        
        private TimerController _timerController;
        private MoveLimitController _moveLimitController;

        private bool _isPaused;
        private bool _gameOverShown;
        
        private List<IGoal> _goals;
        private List<GoalItemView> _goalViews = new();
        private IReadOnlyList<ObstacleDefinition> _currentObstacles;
        private IReadOnlyList<GoalDefinition> _currentGoalDefinitions;

        private JsonSettingsStore _settingsStore;
        private GameSettings _gameSettings;

        public void Start()
        {
            ApplySelectedLevelConfigIfPresent();
            
            var theme = LevelThemeResolver.Resolve(_currentLevelId, level1Background, level2Background, level3Background);
            ApplyTheme(theme);
            
            _progressStore = new JsonProgressStore();
            var progress = _progressStore.Load(); 
            _levelResultStore = new LevelResultStore();
            
            ResetRunStats();
            _consecutiveHintsShown = 0;
            _totalHintsShown = progress.AdaptiveHints.TotalHintsShown;
            _turnsPlayed = progress.TurnsPlayed;
            
            _session = new GameSession();
            var initialSnapshot = _session.StartNewGame(startHeight, startWidth, _currentObstacles);
            boardRenderer.Render(initialSnapshot);
            
            _timerController = new TimerController();
            _moveLimitController = new MoveLimitController();

            if (gameMode == GameModeType.Timer)
            {
                _timerController.StartTimer(totalGameSeconds);
            }
            else
            {
                _moveLimitController.StartMoves(totalMoves);
            }
            RefreshResourceText();
            
            _inputController = new BoardInputController();
            _inputController.CanAcceptInput = () => !turnPlaybackController.IsPlaying && !IsCurrentModeExpired() &&  !_isPaused && !_gameOverShown;
            
            _idleTracker = new IdleTracker();
            _hintController = new HintController(_session, boardRenderer, hintExplanationView);
            
            boardRenderer.CellClicked += HandleBoardCellClicked;
            boardRenderer.CellClicked += _inputController.OnCellClicked;
            _inputController.MoveSubmitted += ApplyMove;
            _inputController.SelectionChanged += boardRenderer.SetSelection;
            
            pausePanel.Hide();
            _isPaused = false;
            
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);

            _settingsStore = new JsonSettingsStore();
            _gameSettings = _settingsStore.Load();

            GameAudioController.Instance.SetMusicVolume(_gameSettings.musicVolume);
            GameAudioController.Instance.PlayGameplayMusic();
            GameAudioController.Instance.SetEffectsVolume(_gameSettings.effectVolume);
            
            RebuildGoals();
            RebuildGoalViews();
            RefreshGoalHud();
        }
        
        private void Update()
        {
            if (_isPaused || _gameOverShown)
            {
                return;
            }
            
            if (gameMode == GameModeType.Timer && !_timerController.IsExpired)
            {
                _timerController.Tick(Time.deltaTime);
                RefreshResourceText();
            }

            if (turnPlaybackController.IsPlaying)
            {
                return;
            }
            
            if (!_gameOverShown &&  _goals.All(g => g.IsCompleted))
            {
                HandleGameResult(GameResultType.Victory);
                _gameOverShown = true;
                return;
            }
            
            if (IsCurrentModeExpired())
            {
                if (!_gameOverShown)
                {
                    var result = GameResultResolver.Resolve(gameMode, true);
                    HandleGameResult(result);
                    _gameOverShown = true;
                }
                return;
            }
            _idleTracker.Tick(Time.deltaTime);
            TryShowIdleHint();
        }
        
        private void ApplyMove(Move move)
        {
            var activeHintWasApplied = _activeHintMove.HasValue && IsSameMove(_activeHintMove.Value, move);

            _activeHintMove = null;
            _hintController.ClearHint();
            _idleTracker.ResetIdle();
            _hintShownForCurrentIdle = false;
            _consecutiveHintsShown = 0;

            var presentation = _session.TryApplyMove(move);
            if (presentation.WasApplied)
            {
                if (activeHintWasApplied)
                {
                    _hintsAppliedThisRun++;
                }

                foreach (var goal in _goals)
                {
                    goal.Apply(presentation.ClearStats);
                }
                
                _scoreThisRun += presentation.ClearStats.TotalClearedCells;
                RefreshGoalHud();

                _turnsPlayed++;
                _turnsUsedThisRun++;
                _clearedCellsThisRun += presentation.ClearStats.TotalClearedCells;
                _destroyedObstaclesThisRun += presentation.ClearStats.DestroyedObstacles;
                _consecutiveHintsShown = 0;

                if (gameMode == GameModeType.Moves)
                {
                    _moveLimitController.ConsumeMove();
                    RefreshResourceText();
                }
                SaveProgress();
            }
            turnPlaybackController.Play(presentation);
        }
        
        private static bool IsSameMove(Move a, Move b)
        {
            return a.From.Equals(b.From) && a.To.Equals(b.To) || a.From.Equals(b.To) && a.To.Equals(b.From);
        }
        
        private void SaveProgress()
        {
            var progress = new PlayerProgress(new AdaptiveHintState(0, _totalHintsShown), _turnsPlayed);
            _progressStore.Save(progress); 
        }
    }
}