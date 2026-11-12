using Match3.Unity.Audio;
using Match3.Unity.GameFlow;
using Match3.Unity.GameMode;
using Match3.Unity.Levels;
using Match3.Unity.Menu;
using Match3.Unity.Save;
using UnityEngine.SceneManagement;

namespace Match3.Unity.Bootstrap
{
    public partial class GameBootstrap
    {
        public void OnRestartButtonClick()
        {
            GameAudioController.Instance.StopResultEffects();
            
            if (_isPaused)
            {
                pausePanel.Hide();
                _isPaused = false;
            }
            
            StartNewGameFlow();
        }

        public void OnNextLevelClicked()
        {
            GameAudioController.Instance.StopResultEffects();
            
            if (!LevelProgression.TryGetNext(_currentLevelId, out var nextLevel))
            {
                OnBackToMenuClicked();
                return;
            }
            
            var config = LevelConfigFactory.Create(nextLevel);;
            SelectedLevelState.Set(config);
            SceneManager.LoadScene("SampleScene");
        }
        
        public void OnBackToMenuClicked()
        {
            GameAudioController.Instance.StopResultEffects();
            
            _hintController.ClearHint();
            _inputController.ClearSelection();
            pausePanel.Hide();
            _isPaused = false;
            _idleTracker.ResetIdle();
            _hintShownForCurrentIdle = false;

            SelectedLevelState.Clear();
            SceneManager.LoadScene("MainMenuScene");
        }
        
        private void StartNewGameFlow()
        {
            GameAudioController.Instance.SetMusicDucked(false);
            
            _hintController.ClearHint();
            _inputController.ClearSelection();
            _idleTracker.ResetIdle();
            _hintShownForCurrentIdle = false;
            _consecutiveHintsShown = 0;
            
            ResetRunStats();
            RebuildGoals();
            RebuildGoalViews();
            RefreshGoalHud();
            
            var snapshot = _session.StartNewGame(startHeight, startWidth, _currentObstacles);
            boardRenderer.RebuildAndRender(snapshot);
            
            if (gameMode == GameModeType.Timer)
            {
                _timerController.StartTimer(totalGameSeconds);
            }
            else
            {
                _moveLimitController.ResetMoves(totalMoves);
            }

            RefreshResourceText();
            
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
            _gameOverShown = false;
        }
        
        private void HandleGameResult(GameResultType result)
        {
            GameAudioController.Instance.SetMusicDucked(true);
            
            SaveLevelResult(result);
            _hintController.ClearHint();
            _inputController.ClearSelection();

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }
            if (defeatPanel != null)
            {
                defeatPanel.SetActive(false);
            }

            switch (result)
            {
                case GameResultType.Defeat:
                    defeatPanel.SetActive(true);
                    GameAudioController.Instance.PlayDefeatSound();
                    break;

                case GameResultType.Victory:
                    victoryPanel.SetActive(true);
                    GameAudioController.Instance.PlayVictorySound();
                    break;
            }
        }
        
        private bool IsCurrentModeExpired()
        {
            return gameMode == GameModeType.Timer ? _timerController.IsExpired : _moveLimitController.IsExpired;
        }
        
        private void ResetRunStats()
        {
            _manualHintsShownThisRun = 0;
            _idleHintsShownThisRun = 0;
            _hintsAppliedThisRun = 0;

            _clearedCellsThisRun = 0;
            _destroyedObstaclesThisRun = 0;
            _scoreThisRun = 0;
            _turnsUsedThisRun = 0;

            _activeHintMove = null;
            _resultSavedForCurrentRun = false;
        }
        
        private void SaveLevelResult(GameResultType result)
        {
            if (_resultSavedForCurrentRun)
            {
                return;
            }

            _resultSavedForCurrentRun = true;
            _levelResultStore.Append(new LevelRunResultDto
            {
                LevelId = _currentLevelId.ToString(),
                Result = result.ToString(),
                TurnsUsed = _turnsUsedThisRun,
                ManualHintsShown = _manualHintsShownThisRun,
                IdleHintsShown = _idleHintsShownThisRun,
                HintsApplied = _hintsAppliedThisRun,
                TotalClearedCells = _clearedCellsThisRun,
                DestroyedObstacles = _destroyedObstaclesThisRun,
                CompletedAtUtc = System.DateTime.UtcNow.ToString("O")
            });
        }
    }
}